using UnityEngine;

public class NoClip : MonoBehaviour
{
    private CharacterController controller;
    private PlayerMovementController movementController;

    private const float gravityConst = -15f;
    private const float noClipConst = 0f;

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
        movementController = gameObject.GetComponent<PlayerMovementController>();
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
            movementController.GravityValue = noClipConst;
            NoClipActive = true;
        }
        else
        { 
            controller.excludeLayers = LayerMask.GetMask("Nothing");
            movementController.GravityValue = gravityConst;
            NoClipActive = false;
        }
    }
}