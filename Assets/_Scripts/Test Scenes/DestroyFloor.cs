using UnityEngine;
using Mirror;

public class DestroyFloor : NetworkBehaviour
{
    [SerializeField] private GameObject room;


    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.O) && isServer)
        {
            GameObject floor = room.transform.GetChild(1).gameObject;
            GameObject.Destroy(floor);
        }
    }

}
