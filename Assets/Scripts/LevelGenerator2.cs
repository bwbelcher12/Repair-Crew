using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator2 : MonoBehaviour
{


    public List<GameObject> possibleRooms = new List<GameObject>();
    public GameObject doorPrefab;
    public GameObject hallPrefab;
    List<Transform> connectionPoints = new List<Transform>();
    List<Transform> rooms = new List<Transform>();
    List<Vector2> grid;

    public int roomCount;
    public int boundsX, boundsY;

    public bool roomsPlaced;

    // Start is called before the first frame update
    void Awake()
    {
        roomsPlaced = false;

        grid = CreateGrid();

        StartCoroutine(GenerateFloor(0));
    }

    private void OnDrawGizmos()
    {
        foreach(Vector2 space in grid)
        {
            Gizmos.DrawSphere(new Vector3(space.x, 0f, space.y), 1f);
        }
    }

    /*
     *Creates one layer of the map by iteratively instantiating new rooms. Rooms are place on the grid
     *and must not overlap with any previously generated rooms. 
     *Rooms are created in passes, with essential rooms being placed in the first pass, filler rooms in
     *the second, doors in the third, and hallways in the fourth.
     */
    IEnumerator GenerateFloor(int floorPos)
    {
        GameObject room;
        Vector3 roomPos;
        int retries = 0;
        for (int i = 0; i < roomCount; i++)
        {
            int gridIndex = Mathf.RoundToInt(Random.Range(0, grid.Count));
            roomPos = new Vector3(grid[gridIndex].x, floorPos, grid[gridIndex].y);
            room = Instantiate(possibleRooms[Random.Range(0, possibleRooms.Count)], roomPos, new Quaternion(Quaternion.identity.x, Quaternion.identity.y, Quaternion.identity.z, Quaternion.identity.w));
            room.transform.Rotate(0f, Mathf.Abs(Random.Range(1, 5) * 90f), 0f);

            //Gives the room several chances to generate. If it cannot find an adequate spot in 10 tries,
            //assume there are no possible spots and break out of the loop.
            if(CheckRoomFootprint(room) == false)
            {
                GameObject.Destroy(room);
                i--;
                retries++;
                if(retries < 10)
                {
                    continue;
                }
            }

            //Resest the retries for the next overlap scenario.
            retries = 0;
            grid.Remove(new Vector2(roomPos.x, roomPos.z));
            rooms.Add(room.transform);
            yield return null;

            /*
             * MOVE TO APPROPRIATE METHOD AFTER DOOR PASS
             * 
            foreach (Transform child in room.transform)
            {
                Transform connectionPoint = GetConnectionPoints(child);

                if (connectionPoint)
                {
                    connectionPoints.Add(connectionPoint);
                }
            }
            
            */
        }

        yield return null;

        roomsPlaced = true;
        
        AddGridPadding(1);

        StartCoroutine(DoorPass(floorPos));

        //DrawLinks();
    }

    //Iterate over each room, removing overlapping walls and generating at least one door per room.
    IEnumerator DoorPass(int floorPos)
    {
        Vector3 localPosition;
        Quaternion localRotation;

        int wallsInRoom;

        List<Vector3> allWallPositions = new List<Vector3>();
        List<GameObject> overlappingWalls = new List<GameObject>();
        List<Transform> wallList = new List<Transform>();
        List<Transform> connectionPoints = new List<Transform>();

        foreach(Transform room in rooms)
        {
            Transform walls = room.transform.Find("Walls");
            wallsInRoom = walls.childCount;

            //Identify overlapping walls and separate them from non-overlapping walls
            foreach(Transform wall in walls)
            {
                if(allWallPositions.Contains(wall.transform.position))
                {
                    overlappingWalls.Add(wall.gameObject);
                }
                else
                {
                    allWallPositions.Add(wall.transform.position);
                    wallList.Add(wall);
                }
            }

            //Remove overlapping walls
            foreach (GameObject wall in overlappingWalls)
            {
                GameObject.Destroy(wall);
            }

            //Ensure at least one door generates per room, with additional doors generating if the random range hits.
            while (wallList.Count > 0)
            {
                if (wallList.Count == 1 || Random.Range(1, 100) >= 70)
                {
                    localPosition = new Vector3(wallList[0].transform.position.x, floorPos, wallList[0].transform.position.z);
                    localRotation = wallList[0].transform.rotation;

                    GameObject.Destroy(wallList[0].gameObject);
                    wallList.RemoveAt(0);

                    GameObject newDoor = GameObject.Instantiate(doorPrefab, localPosition, localRotation, walls.transform);
                    newDoor.transform.Rotate(new Vector3(0f, -90f, 0f));

                    foreach (Transform child in newDoor.transform)
                    {
                        if (child.name == "Connection Point")
                        {
                            Debug.Log("connection point found");
                            Debug.Log(child.transform.position);
                            if (grid.Contains(new Vector2(Mathf.RoundToInt(child.transform.position.x), Mathf.RoundToInt(child.transform.position.z))))
                            {
                                connectionPoints.Add(child);
                                grid.Remove(new Vector2(Mathf.RoundToInt(child.transform.position.x), Mathf.RoundToInt(child.transform.position.z)));
                            }
                        }
                    }
                }
                else
                {
                    wallList.RemoveAt(0);
                }

                yield return null;
            }
        }

        //StartCoroutine(HallPass(floorPos, connectionPoints));
    }

    IEnumerator HallPass(int floorpos, List<Transform> connectionPoints)
    {
        Debug.Log(connectionPoints.Count);
        foreach(Transform point in connectionPoints)
        {
            Debug.Log("hello");
            GameObject.Instantiate(hallPrefab, point.transform.position, point.transform.rotation);
        }

        yield return null;
    }

    void DrawLinks()
    {
        List<Transform> connectedRooms = new List<Transform>();
        
        List<Edge> connections = new List<Edge>();

        //Draw links one room at a time
        foreach (Transform room in rooms)
        {
            List<Transform> localConnectionPoints = new List<Transform>();

            //Add all connection points in the current room to a list
            foreach (Transform child in room.transform)
            {
                Transform connectionPoint = GetConnectionPoints(child);

                if (connectionPoint)
                {
                    localConnectionPoints.Add(connectionPoint);
                }
            }

            Debug.Log(localConnectionPoints.Count);

            //
            List<Transform> usedPoints = new List<Transform>();
            foreach (Transform point in localConnectionPoints)
            {
                Edge shortestPath = new Edge(Vector3.negativeInfinity, Vector3.positiveInfinity);

                foreach (Transform compPoint in connectionPoints)
                {
                    if (point.Equals(compPoint))
                        continue;

                    if (point.transform.parent.Equals(compPoint.transform.parent))
                        continue;

                    if (usedPoints.Contains(compPoint))
                        continue;

                    Edge edge = new Edge(point.transform.position, compPoint.transform.position);

                    edge.Draw(.05f, Color.red);

                    if (shortestPath.Length() > edge.Length())
                    {
                        shortestPath = edge;
                    }

                }
                shortestPath.Draw(999f, Color.green);
                connections.Add(shortestPath);

            }
        }
        foreach(Edge edge in connections)
        {
            edge.Draw();
        }
    }

    IEnumerator DrawConnections()
    {
        //Algoritiom from https://en.wikipedia.org/wiki/Bowyer%E2%80%93Watson_algorithm
        List<Triangle> triangulation = new List<Triangle>(); //List to hold triangls
        Triangle superTrianlge = new Triangle(new Vector3(boundsX * 30f, 0f, 0f), new Vector3(-boundsX * 20f, 0f, boundsY * 30f), new Vector3(-boundsX * 20f, 0f, -boundsY* 30f)); //Create a Super-triangle that is large enough to cotain all points
        triangulation.Add(superTrianlge);

        foreach(Transform point in connectionPoints.ToArray())
        {
            Debug.Log(triangulation.Count);
            List<Triangle> badTriangles = new List<Triangle>();
            foreach (Triangle tri in triangulation)
            {
                Circumcircle circle = CalculateCircumcirlce(tri);
                Debug.Log(circle.ToString());
                if(circle.ContainsPoint(point.position))
                {
                    badTriangles.Add(tri);
                }
            }
            Debug.Log(badTriangles.Count + " bad tris");
            List<Edge> polygon = new List<Edge>();
            foreach(Triangle tri in badTriangles)
            {
                tri.Draw(.05f, Color.red);
                Debug.Log(2);
                Debug.Log(badTriangles.Count);
                Debug.Log(tri.ToString());
                foreach(Edge edge in tri.Edges())
                { 
                    foreach(Triangle triangle in badTriangles)
                    {
                        if (triangle.Equals(tri) && badTriangles.Count > 1)
                            continue;
                        else
                        {
                            foreach(Edge compEdge in triangle.Edges())
                            {
                                if (edge.Equals(compEdge))
                                    continue;
                                else if (polygon.Contains(edge))
                                    continue;
                                else
                                    polygon.Add(edge);
                            }
                        }
                    }
                }
                yield return null;
            }
            foreach(Triangle triangle in badTriangles)
            {
                Debug.Log(3 + " " + badTriangles.Count);
                triangle.Draw(.05F, Color.yellow);
                if(triangulation.Contains(triangle))
                {
                    triangulation.Remove(triangle);
                }
                yield return null;
            }
            Debug.Log(polygon.Count);
            foreach(Edge edge in polygon)
            {
                Triangle newTri = new Triangle(point.position, edge.Point1, edge.Point2);
                triangulation.Add(newTri);
                newTri.Draw(.05f, Color.magenta);
                yield return null;
                //newTri.Draw();
            }
            yield return null;
        }
        foreach(Triangle triangle in triangulation.ToArray())
        {
            foreach(Vector3 vertex in triangle.Vertices())
            {
                foreach(Vector3 superVertex in superTrianlge.Vertices())
                {
                    if (vertex.Equals(superVertex))
                        triangulation.Remove(triangle);
                }
            }
        }
        foreach(Triangle triangle in triangulation)
        {
            triangle.Draw();
        }
    }

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

    public struct Triangle
    { 
        public Triangle(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
        {
            Vertex1 = vertex1;
            Vertex2 = vertex2;
            Vertex3 = vertex3;

            Edge1 = new Edge(vertex1, vertex2);
            Edge2 = new Edge(vertex2, vertex3);
            Edge3 = new Edge(vertex3, vertex1);

        }

        public Vector3 Vertex1 { get; }
        public Vector3 Vertex2 { get; }
        public Vector3 Vertex3 { get; }
        public Edge Edge1, Edge2, Edge3;

        public override string ToString()
        {
            return new string (Vertex1 + ", " + Vertex2 + ", " + Vertex3);
        }

        public void Draw()
        {
            Debug.DrawLine(Vertex1, Vertex2, Color.green, 999f);
            Debug.DrawLine(Vertex2, Vertex3, Color.green, 999f);
            Debug.DrawLine(Vertex3, Vertex1, Color.green, 999f);

        }

        public void Draw(float duration, Color color)
        {
            Debug.DrawLine(Vertex1, Vertex2, color, duration);
            Debug.DrawLine(Vertex2, Vertex3, color, duration);
            Debug.DrawLine(Vertex3, Vertex1, color, duration);

        }

        public Edge Side1()
        {
            Edge edge = new Edge();

            edge.Point1 = Vertex1;
            edge.Point2 = Vertex2;

            return edge;
        }
        public Edge Side2()
        {
            Edge edge = new Edge();

            edge.Point1 = Vertex2;
            edge.Point2 = Vertex3;

            return edge;
        }
        public Edge Side3()
        {
            Edge edge = new Edge();

            edge.Point1 = Vertex3;
            edge.Point2 = Vertex1;

            return edge;
        }
        public List<Edge> Edges()
        {
            List<Edge> edges = new List<Edge>();

            edges.Add(Edge1);
            edges.Add(Edge2);
            edges.Add(Edge3);

            return edges;
        }

        public List<Vector3> Vertices()
        {
            List<Vector3> vertices = new List<Vector3>();

            vertices.Add(Vertex1);
            vertices.Add(Vertex2);
            vertices.Add(Vertex3);

            return vertices;
        }
    }

    public struct Circumcircle
    {
        public Circumcircle(Vector3 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        public Vector3 Center { get; set; }
        public float Radius { get; set; }

        public bool ContainsPoint(Vector3 point)
        {
            if (Vector3.Distance(point, Center) <= Radius)
                return true;
            else
                return false;
        }

        public override string ToString()
        {
            return "Circle of radius " + Radius + " @ " + Center;
        }
    }

    private Circumcircle CalculateCircumcirlce(Triangle triangle)
    {
        Circumcircle circle = new Circumcircle();
        float radius;
        Vector3 center;
        float denominator = triangle.Edge1.Length() * triangle.Edge2.Length() * triangle.Edge3.Length();
        //Debug.Log(denominator);

        float component1 = triangle.Edge1.Length() + triangle.Edge2.Length() + triangle.Edge3.Length();
        //Debug.Log(component1);
        float component2 = -triangle.Edge1.Length() + triangle.Edge2.Length() + triangle.Edge3.Length();
        //Debug.Log(component2);
        float component3 = triangle.Edge1.Length() - triangle.Edge2.Length() + triangle.Edge3.Length();
        //Debug.Log(component3);
        float component4 = triangle.Edge1.Length() + triangle.Edge2.Length() - triangle.Edge3.Length();
        //Debug.Log(component4);

        float numerator = Mathf.Sqrt(component1 * component2 * component3 * component4);
        //Debug.Log(numerator);
        
        center = new Vector3((triangle.Vertex1.x + triangle.Vertex2.x + triangle.Vertex3.x) / 3, 0f, (triangle.Vertex1.z + triangle.Vertex2.z + triangle.Vertex3.z) / 3);

        radius = denominator / numerator;

        circle.Radius = radius;
        circle.Center = center;


        return circle;
    }

    public struct Edge
    {
        public Edge (Vector3 point1, Vector3 point2)
        {
            Point1 = point1;
            Point2 = point2;
        }
        
        public Vector3 Point1 { get; set; }
        public Vector3 Point2 { get; set; }

        public float Length()
        {
            return Mathf.Abs(Vector3.Distance(Point1, Point2));
        }

        public void Draw()
        {
            Debug.DrawLine(Point1, Point2, Color.red, 999f);
        }

        public void Draw(float duration, Color color)
        {
            Debug.DrawLine(Point1, Point2, color, duration);
        }
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
    }
}