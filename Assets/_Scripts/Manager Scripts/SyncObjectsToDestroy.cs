using Mirror;
using System.Collections.Generic;

public class SyncObjectsToDestroy : NetworkBehaviour
{
    public List<int> badWallIndicies;

    [SyncVar(hook = nameof(DisableObjects))]
    bool isEnabled = true;
    public bool IsEnabled 
    { 
        get => isEnabled;
        set => isEnabled = value;
    }

    public void DisableObjects(bool oldValue, bool newValue)
    {
        foreach (int index in badWallIndicies)
        {
            this.transform.Find("Walls").GetChild(index).gameObject.SetActive(false);
        }
    }
}
