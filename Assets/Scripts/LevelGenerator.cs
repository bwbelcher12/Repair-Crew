using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public List<GameObject> singleDoorRooms = new List<GameObject>();
    public List<GameObject> doubleDoorRooms = new List<GameObject>();
    public List<GameObject> tripleDoorRooms = new List<GameObject>();
    public List<GameObject> quadrupleDoorRooms = new List<GameObject>();
    public List<GameObject> possibleRooms = new List<GameObject>();
    public List<GameObject> halls = new List<GameObject>();
    [SerializeField] int depth;
    [SerializeField] GameObject startingRoom;

    // Start is called before the first frame update
    void Awake()
    {
        GenerateWorld();
    }


    private void GenerateWorld()
    {
        GameObject room;

        room = Instantiate(startingRoom, Vector3.zero, Quaternion.identity);

        StartCoroutine(BuildRooms(depth, room));
    }

    private Transform GetConnectionPoints(Transform parent)
    {         
        if(parent == null)
        {
            return null;
        }

        if (parent.name == "Connection Point")
        {
            return parent;
        }

        foreach(Transform child in parent)
        {
            if(child.name == "Connection Point")
            {
                return child;
            }
            else
            {
                Transform found = GetConnectionPoints(child);
                if(found != null)
                {
                    return found;
                }
            }
        }
        return null;
    }

    IEnumerator BuildRooms(int iteration, GameObject startingRoom)
    {

        if (iteration <= 0)
        {
            yield break;
        }

        List<Transform> connectionPoints = new List<Transform>();

        foreach (Transform child in startingRoom.transform)
        {
            Transform connectionPoint = GetConnectionPoints(child);

            if (connectionPoint)
            {
                connectionPoints.Add(connectionPoint);
            }
        }

        foreach(Transform connection in connectionPoints)
        {
            GameObject room;
            if(iteration == 1)
            {
                if (connection == null)
                {
                    continue;
                }

                RaycastHit hit;

                if (Physics.Raycast(connection.position, -1f * (connection.parent.transform.position - connection.transform.position).normalized, out hit, 5.5f))
                {
                    Debug.Log("OVERLAP AT" + hit.point);
                    Debug.DrawLine(connection.position, hit.point, Color.red, 100f);
                    continue;
                }
                else
                {
                    room = Instantiate(singleDoorRooms[0], connection.position + (connection.position - connection.transform.parent.transform.position), Quaternion.identity);
                }
            }
            else
            {
                if (connection == null)
                {
                    continue;
                }

                RaycastHit hit;

                if (Physics.Raycast(connection.position, -1f * (connection.parent.transform.position - connection.transform.position).normalized, out hit, 5.5f))
                {
                    Debug.Log("OVERLAP AT" + hit.point);
                    Debug.DrawLine(connection.position, hit.point, Color.red, 100f);
                    continue;

                }
                else
                {
                    room = Instantiate(possibleRooms[Mathf.RoundToInt(Random.Range(0, possibleRooms.Count))], connection.position + (connection.position - connection.transform.parent.transform.position), Quaternion.identity);
                }
            }
            room.transform.LookAt(connection, Vector3.up);

            //removes the overlapping connection to prevent backwards room generation
            foreach(Transform connection2 in room.transform)
            {
                if(connection.position == connection2.position)
                {
                    GameObject.Destroy(connection2.gameObject);
                    //connectionPoints.Remove(connection);
                    GameObject.Destroy(connection.gameObject);
                }    
            }
            yield return new WaitForSeconds(1f);

            Debug.Log(iteration);
            StartCoroutine(BuildRooms(iteration - 1, room));
        }
    }

}
