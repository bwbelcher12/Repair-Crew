using UnityEngine;
using Mirror;
public class MirrorPlayerScript : NetworkBehaviour
{
    private NetworkTransformReliable networkTransform;

    private void Start()
    {
        networkTransform = transform.GetComponent<NetworkTransformReliable>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!networkTransform.isOwned)
        {
            return;
        }

        Move();
    }

    void Move()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            transform.Translate(Vector3.forward);
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            transform.Translate(Vector3.back);
        }
        if(Input.GetKeyDown(KeyCode.A))
        {
            transform.Translate(Vector3.left);
        }
        if(Input.GetKeyDown(KeyCode.D))
        {
            transform.Translate(Vector3.right);
        }
    }
}
