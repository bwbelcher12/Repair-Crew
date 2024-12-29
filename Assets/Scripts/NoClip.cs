using UnityEngine;

public class NoClip : MonoBehaviour
{
    private CharacterController controller;

    [SerializeField] private bool noClipActive;

    public bool NoClipActive 
    {
        get => noClipActive;
        set => noClipActive = value;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
       if (Input.GetKeyDown(KeyCode.X))
        {
            ToggleNoClip();
        }
    }

    private void ToggleNoClip()
    {
        if(!noClipActive)
        { 
            controller.excludeLayers = Physics.AllLayers;
            NoClipActive = true;
        }
        else
        { 
            controller.excludeLayers = LayerMask.GetMask("Nothing");
            NoClipActive = false;
        }
    }
}