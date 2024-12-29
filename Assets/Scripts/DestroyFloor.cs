using UnityEngine;
using Mirror;

public class DestroyFloor : NetworkBehaviour
{
    [SerializeField] private GameObject room;

    [SyncVar(hook = nameof(HookTest))]
    [SerializeField] bool floorEnabled = true;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.O) && isServer)
        {
            GameObject floor = room.transform.GetChild(1).gameObject;
            GameObject.Destroy(floor);
            floorEnabled = false;
        }
    }

    void HookTest(bool oldState, bool newState)
    {
        Debug.Log("hello");
        if(newState == false)
        {
            GameObject floor = room.transform.GetChild(1).gameObject;
            GameObject.Destroy(floor);
        }
    }
}
