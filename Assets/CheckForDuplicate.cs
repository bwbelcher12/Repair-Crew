using UnityEngine;

public class CheckForDuplicate : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Object[] objects = FindObjectsByType(typeof(GameObject), FindObjectsSortMode.InstanceID);
        foreach(GameObject obj in objects)
        {
            if(obj.transform.name == transform.name && obj.transform != transform)
            {
                GameObject.Destroy(this.gameObject);
            }
        }
    }
}
