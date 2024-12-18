using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomMover : MonoBehaviour
{
    public bool doneMoving = false;

    public int roomWidth, roomHeight, roomDepth;

    private Vector3 collisionNormal;
    
    private GameObject levelGeneratorObject;
    private LevelGenerator2 levelGeneratorScript;


    private void Awake()
    {
        levelGeneratorObject = GameObject.FindGameObjectWithTag("Level Generator");
        levelGeneratorScript = levelGeneratorObject.GetComponent<LevelGenerator2>();
    }

    private void FixedUpdate()
    {
        if (levelGeneratorScript.roomsPlaced)
        {
            GameObject.Destroy(transform.gameObject);
        }

    }

    /*
    private void OnCollisionStay(Collision collision)
    {

        if (collision.transform.parent == transform.parent)
        {
            return;
        }

        if (collision.transform.tag == "RoomBounds" )
        {
            collisionNormal = (transform.parent.position - collision.transform.parent.position).normalized;
            collisionNormal = new Vector3(Mathf.RoundToInt(collisionNormal.x), Mathf.RoundToInt(collisionNormal.y), Mathf.RoundToInt(collisionNormal.z));
            transform.parent.position += collisionNormal * 5f;

            doneMoving = false;
        }
    }
    */
}
