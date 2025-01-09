using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;

public class LevelGenerator : NetworkBehaviour
{
    [SerializeField] private bool doDrawGrizoms = true;

    public List<GameObject> possibleRooms = new List<GameObject>();
    public List<GameObject> essentialRooms = new List<GameObject>();
    public GameObject doorPrefab;
    public GameObject straightHall;
    public GameObject lHall;
    public GameObject tHall;
    public GameObject crossHall;
    public GameObject capHall;


    List<Transform> connectionPoints = new List<Transform>();
    List<Vector3> connectionPointPositions = new List<Vector3>();
    List<Vector3> roomPositions = new List<Vector3>();
    List<Vector2> grid;
    List<Vector3> hallwayPositions = new List<Vector3>();
    List<Vector3> tempNeighborSpaces = new List<Vector3>();
    List<Transform> doorWalls = new List<Transform>();

    List<GameObject> overlappingWalls = new List<GameObject>();

    [SyncVar]
    [SerializeField] List<Transform> rooms = new List<Transform>();

    //Lists for determining wall placement
    //*****************************************************************************************
    List<int> wallPositions = new List<int> { 0, 0, 0, 0 };

    List<int> crossHallPositions = new List<int> { 1, 1, 1, 1 };

    List<int> tHallNorth = new List<int> { 1, 0, 1, 1 };
    List<int> tHallSouth = new List<int> { 1, 1, 1, 0 };
    List<int> tHallEast = new List<int> { 1, 1, 0, 1 };
    List<int> tHallWest = new List<int> { 0, 1, 1, 1 };

    List<int> lHallNorthEast = new List<int> { 1, 0, 0, 1 };
    List<int> lHallSouthWest = new List<int> { 0, 1, 1, 0 };
    List<int> lHallNorthWest = new List<int> { 1, 1, 0, 0 };
    List<int> lHallSouthEast = new List<int> { 0, 0, 1, 1 };

    List<int> straightHallNorthSouth = new List<int> { 1, 0, 1, 0 };
    List<int> straightHallEastWest = new List<int> { 0, 1, 0, 1 };

    List<int> capHallNorth = new List<int> { 1, 0, 0, 0 };
    List<int> capHallSouth = new List<int> { 0, 0, 1, 0 };
    List<int> capHallEast = new List<int> { 0, 0, 0, 1 };
    List<int> capHallWest = new List<int> { 0, 1, 0, 0 };
    //*****************************************************************************************

    [SerializeField] private int roomCount;
    [SerializeField] private int boundsX, boundsY;

    //When this object starts, we must wait for all clients to be connected
    //and ready before generating the world. If we don't some of the data 
    //about doors will be lost, and clients that connect late won't know 
    //which walls need to be destroyed.
    private void Start()
    {
        if(isServer)
        {
            StartCoroutine(WaitForPlayers());
        }
    }

    private void Update()
    {
        //MOVE THIS FUNCTION CALL TO THE END OF LEVEL GENERATION
        if(Input.GetKeyDown(KeyCode.O))
        {
            CmdDestroyExtraWalls();
        }
    }

    private void OnDrawGizmos()
    {
        if (!doDrawGrizoms) { return; }

        foreach (Vector2 space in grid)
        {
            Gizmos.DrawSphere(new Vector3(space.x, 0f, space.y), 1f);
        }
        

        Gizmos.color = Color.red;

        foreach (Vector3 position in hallwayPositions)
        {
            Gizmos.DrawSphere(position, 1f);
        }
    }

    //-------------------------------------------------------
    //CORUTINES
    //-------------------------------------------------------

    //Loops through all connections until each connection is ready.
    //When all connections are ready, begin the level generation process.
    private IEnumerator WaitForPlayers()
    {
        bool allPlayersReady = false;

        while (!allPlayersReady)
        {
            for (int i = 0; i < NetworkServer.connections.Count(); i++)
            {
                allPlayersReady = true;
                if (!NetworkServer.connections[i].isReady)
                {
                    allPlayersReady = false;
                }
                yield return null;
            }
        }
        GenerateLevel();
        StopCoroutine(WaitForPlayers());
    }

    //-------------------------------------------------------
    //SERVER FUNCTIONS
    //-------------------------------------------------------

    [Server]
    public void GenerateLevel()
    {
        grid = CreateGrid();
        GenerateFloor(0);
    }

    //This may be unnecessary now, but it is nice to have around.
    [Server]
    public void ClearLevel()
    {
        connectionPoints.Clear();
        connectionPointPositions.Clear();
        rooms.Clear();
        roomPositions.Clear();
        grid.Clear();
        hallwayPositions.Clear();
        tempNeighborSpaces.Clear();
        doorWalls.Clear();
        overlappingWalls.Clear();
    }

    //-------------------------------------------------------
    //CLIENTRPC FUNCTIONS
    //-------------------------------------------------------

    //These functions are needed due to Mirror's no-nested-NetworkIdentity policy.
    //Since the walls generate as children of a room prefab, they need to be destroyed 
    //locally. 

    [ClientRpc]
    private void RpcAddToOverlappingWalls(int wallIndex, GameObject room)
    {
        room.GetComponent<SyncObjectsToDestroy>().badWallIndicies.Add(wallIndex);
    }

    [ClientRpc]
    public void CmdDestroyExtraWalls()
    {
        foreach (Transform room in rooms)
        {
            room.GetComponent<SyncObjectsToDestroy>().IsEnabled = false;
        }
    }

    /*
     *Creates one layer of the map by iteratively instantiating new rooms. Rooms are place on the grid
     *and must not overlap with any previously generated rooms. 
     *Rooms are created in passes, with essential rooms being placed in the first pass, filler rooms in
     *the second, doors in the third, and hallways in the fourth.
     */
    private void GenerateFloor(int floorPos)
    {
        //This is called first so that we guarantee all the essential rooms on each floor generate properly.
        GenerateEssentialRooms(floorPos);

        //Loop variables holding information about our rooma and where it should be spawned.
        GameObject room;
        Vector3 roomSpawnPos;
        int gridIndex;
        
        //An iterator 
        int retries = 0;

        for (int i = 0; i < roomCount; i++)
        {
            //Identify an empty space in our grid and create a room there. Since space are removed
            //from the grid list 
            gridIndex = Mathf.RoundToInt(Random.Range(0, grid.Count));
            roomSpawnPos = new Vector3(grid[gridIndex].x, floorPos, grid[gridIndex].y);
            room = Instantiate(possibleRooms[Random.Range(0, possibleRooms.Count)], roomSpawnPos, new Quaternion(Quaternion.identity.x, Quaternion.identity.y, Quaternion.identity.z, Quaternion.identity.w));
            room.transform.Rotate(0f, Mathf.Abs(Random.Range(1, 5) * 90f), 0f);

            //Gives the room several chances to generate. If it cannot find an adequate spot in 10 tries,
            //assume there are no possible spots and break out of the current loop iteration. Due to the 
            //grid system, this really only happends with rooms that are not 1x1 cells.
            if(CheckRoomFootprint(room) == false)
            {
                //Destroy the overlapping room and reset the iterator. 
                GameObject.Destroy(room);
                i--;

                //If we have tried more than 10 times, we can assume this room won't be able to spawn 
                //and we continue on in the loop. This could cause the map to generate with fewer 
                //filler rooms than specified, but that isn't really a big deal.
                retries++;
                if(retries < 10)
                {
                    continue;
                }
            }

            //Resest the retries for the next overlap scenario.
            retries = 0;

            //Since we now know our room will generate without overlap, it can claim this position in 
            //the grid and be added to the room list. 
            grid.Remove(new Vector2(roomSpawnPos.x, roomSpawnPos.z));
            rooms.Add(room.transform);
            roomPositions.Add(FixVector3Floats(room.transform.position));

            //Each non-standard room needs to have all the cells it takes up added to the room positions
            //list.
            foreach (Transform child in room.transform)
            {
                if (child.name.Contains("Footprint"))
                {
                    roomPositions.Add(FixVector3Floats(child.position));
                }
            }
        }

        //Since essential rooms are added to the rooms list and spawned on clients in
        //GenerateEssentailRooms(), we start iterating through the rooms after their entries.
        //Once all rooms on a floor have been placed on the server, they can be spawned on each client.
        int j = essentialRooms.Count();
        while (j < rooms.Count)
        {
            NetworkServer.Spawn(rooms[j].gameObject);
            j++;
        }

        //Adds new gridspaces along the perimeter of the grid that hallways can use.
        AddGridPadding(1);

        //Once all our rooms are placed, we can begin adding doors to them.
        DoorPass(floorPos);
    }

    //This function is almost identical to GenerateRooms() with a few special cases carved out.
    private void GenerateEssentialRooms(int floorPos)
    {
        GameObject room;
        Vector3 roomPos;
        int retries = 0;
        for (int i = 0; i <= essentialRooms.Count() - 1; i++)
        {
            if (i == 0)
            {
                roomPos = new Vector3((boundsX * 10) + 10, floorPos, (boundsY / 2) * 10);
                room = Instantiate(essentialRooms[i], roomPos, new Quaternion(Quaternion.identity.x, Quaternion.identity.y, Quaternion.identity.z, Quaternion.identity.w));
                room.transform.Rotate(0f, Mathf.Abs(Random.Range(1, 5) * 90f), 0f);
            }
            else
            {
                int gridIndex = Mathf.RoundToInt(Random.Range(0, grid.Count));
                roomPos = new Vector3(grid[gridIndex].x, floorPos, grid[gridIndex].y);
                room = Instantiate(essentialRooms[i], roomPos, new Quaternion(Quaternion.identity.x, Quaternion.identity.y, Quaternion.identity.z, Quaternion.identity.w));
                room.transform.Rotate(0f, Mathf.Abs(Random.Range(1, 5) * 90f), 0f);
            }
            //Gives the room several chances to generate. If it cannot find an adequate spot in 10 tries,
            //assume there are no possible spots and break out of the loop.
            if (CheckRoomFootprint(room) == false)
            {
                GameObject.Destroy(room);
                i--;
                retries++;
                if (retries < 10)
                {
                    continue;
                }
            }

            //Resest the retries for the next overlap scenario.
            retries = 0;
            grid.Remove(new Vector2(roomPos.x, roomPos.z));
            rooms.Add(room.transform);
            roomPositions.Add(FixVector3Floats(room.transform.position));
            NetworkServer.Spawn(room);

            foreach (Transform child in room.transform)
            {
                if (child.name.Contains("Footprint"))
                {
                    roomPositions.Add(FixVector3Floats(child.position));
                }
            }
        }
    }

    //Iterate over each room, removing overlapping walls and generating at least one door per room.
    private void DoorPass(int floorPos)
    {
        Vector3 localPosition;
        Quaternion localRotation;

        int wallsInRoom;

        List<Vector3> allWallPositions = new List<Vector3>();
        List<Transform> wallList = new List<Transform>();
        List<Transform> connectionPoints = new List<Transform>();

        foreach(Transform room in rooms)
        {
            Transform walls = room.transform.Find("Walls");
            wallsInRoom = walls.childCount;

            //Identify overlapping walls and separate them from non-overlapping walls
            foreach (Transform wall in walls)
            {
                if (!allWallPositions.Contains(FixVector3Floats(wall.transform.position)))
                {
                    allWallPositions.Add(FixVector3Floats(wall.transform.position));
                    wallList.Add(wall);
                }
                else
                {
                    RpcAddToOverlappingWalls(wall.transform.GetSiblingIndex(), room.gameObject);
                }
            }
            //Ensure at least one door generates per room, with additional doors generating if the random range hits.
            while (wallList.Count > 0)
            {
                //Special case for making sure the player start room doesn't generate a door on it's outermost wall.
                if (wallList[0].position.x > (boundsX * 10) + 10)
                {
                    wallList.RemoveAt(0);
                    continue;
                }

                //Another special case for the starting room to make sure at least one door (and often two doors) generate.
                //This can happen because the above statement has a chance of making the starter room miss the wallList.Count == 1 conditon later.
                if (room.transform.name == "PlayerStartingRoom(Clone)" && wallList.Count == 2)
                {
                    localPosition = new Vector3(wallList[0].transform.position.x, floorPos, wallList[0].transform.position.z);
                    localRotation = wallList[0].transform.rotation;

                    RpcAddToOverlappingWalls(wallList[0].transform.GetSiblingIndex(), room.gameObject);
                    wallList.RemoveAt(0);

                    GameObject newDoor = GameObject.Instantiate(doorPrefab, localPosition + (Vector3.up * 1.5f), localRotation);

                    NetworkServer.Spawn(newDoor);

                    foreach (Transform child in newDoor.transform)
                    {
                        if (child.name == "Connection Point")
                        {
                            //connectionPoints.Add(child);
                            if (grid.Contains(new Vector2(Mathf.RoundToInt(child.transform.position.x), Mathf.RoundToInt(child.transform.position.z))))
                            {
                                connectionPoints.Add(child);
                                connectionPointPositions.Add(FixVector3Floats(child.position));
                                grid.Remove(new Vector2(Mathf.RoundToInt(child.transform.position.x), Mathf.RoundToInt(child.transform.position.z)));
                            }
                        }
                    }
                    continue;
                }

                //For all other rooms, ensure at least one door is generated, with each other wall having a 30% chance to generate a door.
                if (wallList.Count == 1 || Random.Range(1, 100) >= 70)
                {
                    localPosition = new Vector3(wallList[0].transform.position.x, floorPos, wallList[0].transform.position.z);
                    localRotation = wallList[0].transform.rotation;

                    RpcAddToOverlappingWalls(wallList[0].transform.GetSiblingIndex(), room.gameObject);
                    wallList.RemoveAt(0);

                    GameObject newDoor = GameObject.Instantiate(doorPrefab, localPosition + (Vector3.up * 1.5f), localRotation);

                    NetworkServer.Spawn(newDoor);

                    foreach (Transform child in newDoor.transform)
                    {
                        if (child.name == "Connection Point")
                        {
                            //connectionPoints.Add(child);
                            if (grid.Contains(new Vector2(Mathf.RoundToInt(child.transform.position.x), Mathf.RoundToInt(child.transform.position.z))))
                            {
                                connectionPoints.Add(child);
                                connectionPointPositions.Add(FixVector3Floats(child.position));
                                grid.Remove(new Vector2(Mathf.RoundToInt(child.transform.position.x), Mathf.RoundToInt(child.transform.position.z)));
                            }
                        }
                    }
                }
                else
                {
                    wallList.RemoveAt(0);
                }
            }
        }
        HallPass(floorPos, connectionPoints);
    }

    //First, each hallway is plotted, storing the grid positions for each hall piece in hallwayPositions.
    //Then we iterate over each of those positions and instantiate the proper hall peice there.
    private void HallPass(int floorpos, List<Transform> connectionPoints)
    {
        int i = 0;

        while (i < connectionPoints.Count)
        {
            ConstructHall(FixVector3Floats(connectionPoints[i].position), FixVector3Floats(connectionPoints[Random.Range(0, connectionPoints.Count)].position));
            i++;
        }

        foreach (Vector3 position in hallwayPositions)
        {
            InstatiateHall(FixVector3Floats(position));
        }
    }

    //Fucntion used to find "Connection Point" objects that are nested in a parent object.
    //"Connection Points" are attached to doorways and indicate to the hall pass where halls should start.
    private Transform GetConnectionPoints(Transform parent)
    {
        if (parent == null)
        {
            return null;
        }

        if (parent.name == "Connection Point")
        {
            return parent;
        }

        foreach (Transform child in parent)
        {
            if (child.name == "Connection Point")
            {
                return child;
            }
            else
            {
                Transform found = GetConnectionPoints(child);
                if (found != null)
                {
                    return found;
                }
            }
        }
        return null;
    }

    //Fills the list used to select room generation locations from
    private List<Vector2> CreateGrid()
    {
        List<Vector2> grid = new List<Vector2>();
        int i = 1;
        while (i <= boundsX) 
        {
            int j = 1;
            while (j <= boundsY)
            {
                grid.Add(new Vector2(i * 10f, j * 10f));
                j++;
            }
            i++;
        }

        return grid;
    }

    //Validates that non-1x1 rooms don't overlap with other rooms.
    private bool CheckRoomFootprint(GameObject room)
    {
        List<Vector2> tempPositions = new List<Vector2>();
        foreach (Transform child in room.transform)
        {
            if(child.name.Contains("Footprint"))
            {
                if(grid.Contains(new Vector2(child.transform.position.x, child.transform.position.z)))
                {
                    tempPositions.Add(new Vector2(child.position.x, child.position.z));
                }
                else
                {
                    return false;
                }
            }
        }

        foreach (Vector2 pos in tempPositions)
        {
            grid.Remove(pos);
        }

        return true;
    }

    //Adds the padding on the outside of the grid used by the starting room and some hallways.
    private void AddGridPadding (int paddingSize)
    {
        int i = -paddingSize + 1;
        while (i <= 0)
        {
            int j = -paddingSize + 1;
            while (j < boundsY + paddingSize + 1)
            {
                grid.Add(new Vector2(i * 10f, j * 10f));
                j++;
            }
            i++;
        }

        i = boundsX + 1;
        while (i <= boundsX + paddingSize)
        {
            int j = -paddingSize + 1;
            while (j < boundsY + paddingSize + 1)
            {
                grid.Add(new Vector2(i * 10f, j * 10f));
                j++;
            }
            i++;
        }

        i = 0;
        while (i <= boundsX) 
        {
            int j = boundsY + 1;
            while (j <= boundsY + paddingSize)
            {
                grid.Add(new Vector2(i * 10f, j * 10f));
                j++;
            }
            i++;
        }

        i = 1;
        while (i <= boundsX)
        {
            int j = -paddingSize +1;
            while (j <= 0)
            {
                grid.Add(new Vector2(i * 10f, j * 10f));
                j++;
            }
            i++;
        }

        GameObject startingRoom = GameObject.Find("PlayerStartingRoom(Clone)");
        grid.Remove(new Vector2(startingRoom.transform.position.x, startingRoom.transform.position.z));
    }

    //This probably isn't the best way to do this, but it does give a good result so it will stay for now.
    //Essentially, two connection points are passed to the function, the shortest path along the grid between
    //them is added to the hallwayPositions list. It is recusive, and declaring too many variables in the 
    //function body was causing some stack overflows, so be aware of that.
    private void ConstructHall (Vector3 connectionpoint1, Vector3 connectionpoint2)
    {
        tempNeighborSpaces.Clear();

        if (hallwayPositions.Contains(connectionpoint1) == false)
        {
            hallwayPositions.Add(connectionpoint1);
        }

        getNeighborSpaces(connectionpoint1);

        if(tempNeighborSpaces.Count == 0)
        {
            return;
        }

        Vector3 closestPoint = tempNeighborSpaces[0];


        foreach (Vector3 space in tempNeighborSpaces)
        {
            if (roomPositions.Contains(space))
            {
                continue;
            }
            if(Mathf.Abs(Vector3.Distance(space, connectionpoint2)) <= Mathf.Abs(Vector3.Distance(closestPoint, connectionpoint2)))
            {
                closestPoint = space;
            }
        }

        if(closestPoint == connectionpoint2)
        {
            return;
        }

        if(hallwayPositions.Contains(closestPoint) == false)
        {
            hallwayPositions.Add(closestPoint);
        }
        else
        {
            return;
        }

        ConstructHall(closestPoint, connectionpoint2);
    }

    //Finds neighboring positions on the grid. Grid spacing is currently hardcoded at 10, but that shouldn't
    //need to change.
    private void getNeighborSpaces(Vector3 position)
    {

        Vector3 roundedPosition = FixVector3Floats(position);


        if (grid.Contains(new Vector2(roundedPosition.x + 10, roundedPosition.z)))
        {
            tempNeighborSpaces.Add(new Vector3(roundedPosition.x + 10, roundedPosition.y, roundedPosition.z));
        }

        if(grid.Contains(new Vector2(roundedPosition.x - 10, roundedPosition.z)))
        {
            tempNeighborSpaces.Add(new Vector3(roundedPosition.x - 10, roundedPosition.y, roundedPosition.z));
        }
        
        if(grid.Contains(new Vector2(roundedPosition.x, roundedPosition.z + 10)))
        {
            tempNeighborSpaces.Add(new Vector3(roundedPosition.x, roundedPosition.y, roundedPosition.z + 10));
        }

        if(grid.Contains(new Vector2(roundedPosition.x, roundedPosition.z - 10)))
        {
            tempNeighborSpaces.Add(new Vector3(roundedPosition.x, roundedPosition.y, roundedPosition.z - 10));
        }
    }

    //Populate an array with information regarding walls in surrounding tiles. This is used by InstantiateHall()
    //to determine which prefab to use.
    private List<int> determineWallPositions(Vector3 position)
    {

        wallPositions = new List<int> { 0, 0, 0, 0 }; 

        Vector3 roundedPosition = new Vector3(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), Mathf.RoundToInt(position.z));

        if (hallwayPositions.Contains(new Vector3(roundedPosition.x + 10, roundedPosition.y, roundedPosition.z)))
        {
            wallPositions[0] = 1;
        }

        if (hallwayPositions.Contains(new Vector3(roundedPosition.x - 10, roundedPosition.y, roundedPosition.z)))
        {
            wallPositions[2] = 1;
        }

        if (hallwayPositions.Contains(new Vector3(roundedPosition.x, roundedPosition.y, roundedPosition.z + 10)))
        {
            wallPositions[1] = 1;
        }

        if (hallwayPositions.Contains(new Vector3(roundedPosition.x, roundedPosition.y, roundedPosition.z - 10)))
        {
            wallPositions[3] = 1;
        }

        if (connectionPointPositions.Contains(position))
        {
            if (roomPositions.Contains(new Vector3(roundedPosition.x + 10, roundedPosition.y, roundedPosition.z)))
            {
                wallPositions[0] = 1;
            }

            if (roomPositions.Contains(new Vector3(roundedPosition.x - 10, roundedPosition.y, roundedPosition.z)))
            {
                wallPositions[2] = 1;
            }

            if (roomPositions.Contains(new Vector3(roundedPosition.x, roundedPosition.y, roundedPosition.z + 10)))
            {
                wallPositions[1] = 1;
            }

            if (roomPositions.Contains(new Vector3(roundedPosition.x, roundedPosition.y, roundedPosition.z - 10)))
            {
                wallPositions[3] = 1;
            }
        }

        return wallPositions;
    }

  
    //Long conditional that should probably be a switch case. THis determinies which hall prefab + rotation 
    //should be used based on the contents of wallPositions.
    private void InstatiateHall(Vector3 position)
    {
        List<int> wallPositions = determineWallPositions(position);
        GameObject hall = null;

        if (wallPositions.SequenceEqual(crossHallPositions))
        {
            hall = GameObject.Instantiate(crossHall, position, Quaternion.identity);
        }
        else if(wallPositions.SequenceEqual(tHallWest))
        {
            hall = GameObject.Instantiate(tHall, position, Quaternion.identity);
        }
        else if (wallPositions.SequenceEqual(tHallEast))
        {
            hall = GameObject.Instantiate(tHall, position, Quaternion.identity);
            hall.transform.Rotate(0f, 180f, 0f);
        }
        else if(wallPositions.SequenceEqual(tHallNorth))
        {
            hall = GameObject.Instantiate(tHall, position, Quaternion.identity);
            hall.transform.Rotate(0f, 270f, 0f);
        }
        else if(wallPositions.SequenceEqual(tHallSouth))
        {
            hall = GameObject.Instantiate(tHall, position, Quaternion.identity);
            hall.transform.Rotate(0f, 90f, 0f);
        }
        else if(wallPositions.SequenceEqual(lHallNorthEast))
        {
            hall = GameObject.Instantiate(lHall, position, Quaternion.identity);
            hall.transform.Rotate(0f, 180f, 0f);
        }
        else if (wallPositions.SequenceEqual(lHallSouthWest))
        {
            hall = GameObject.Instantiate(lHall, position, Quaternion.identity);
            hall.transform.Rotate(0f, 0f, 0f);
        }
        else if (wallPositions.SequenceEqual(lHallNorthWest))
        {
            hall = GameObject.Instantiate(lHall, position, Quaternion.identity);
            hall.transform.Rotate(0f, 90f, 0f);
        }
        else if (wallPositions.SequenceEqual(lHallSouthEast))
        {
            hall = GameObject.Instantiate(lHall, position, Quaternion.identity);
            hall.transform.Rotate(0f, 270f, 0f);
        }
        else if (wallPositions.SequenceEqual(straightHallNorthSouth))
        {
            hall = GameObject.Instantiate(straightHall, position, Quaternion.identity);
            hall.transform.Rotate(0f, 90f, 0f);
        }
        else if (wallPositions.SequenceEqual(straightHallEastWest))
        {
            hall = GameObject.Instantiate(straightHall, position, Quaternion.identity);
            hall.transform.Rotate(0f, 0f, 0f);
        }
        else if (wallPositions.SequenceEqual(capHallWest))
        {
            hall = GameObject.Instantiate(capHall, position, Quaternion.identity);
            hall.transform.Rotate(0f, 180f, 0f);
        }
        else if (wallPositions.SequenceEqual(capHallEast))
        {
            hall = GameObject.Instantiate(capHall, position, Quaternion.identity);
            hall.transform.Rotate(0f, 0f, 0f);
        }
        else if (wallPositions.SequenceEqual(capHallNorth))
        {
            hall = GameObject.Instantiate(capHall, position, Quaternion.identity);
            hall.transform.Rotate(0f, 270f, 0f);
        }
        else if (wallPositions.SequenceEqual(capHallSouth))
        {
            hall = GameObject.Instantiate(capHall, position, Quaternion.identity);
            hall.transform.Rotate(0f, 90f, 0f);
        }
        else
        {
            hall = GameObject.Instantiate(straightHall, position, Quaternion.identity);
        }

        NetworkServer.Spawn(hall);
    }

    //Simple function that fixes some floating point errors that were being caused after rotating some prefabs.
    private Vector3 FixVector3Floats(Vector3 position)
    {
        Vector3 fixedVector3 = new Vector3(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), Mathf.RoundToInt(position.z));

        return fixedVector3;
    }

}