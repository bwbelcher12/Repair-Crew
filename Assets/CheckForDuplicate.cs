using UnityEngine;

public class CheckForDuplicate : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Object[] objects = FindObjectsByType(typeof(GameObject), FindObjectsSortMode.None);
        foreach(GameObject obj in objects)
        {
            if(obj.transform.name == transform.name && obj.transform != transform)
            {
                GameObject.Destroy(gameObject);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
