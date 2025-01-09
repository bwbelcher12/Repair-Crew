using UnityEngine;
using Mirror;

public class PlayerAudioManager : NetworkBehaviour
{

    [SerializeField] Camera playerCam;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(!isLocalPlayer)
        {
            return;
        }

        AudioListener playerListener = playerCam.GetComponent<AudioListener>();

        EnableAudioListener(playerListener);
    }

    private void EnableAudioListener(AudioListener listener)
    {
        listener.enabled = true;
    }

    private void DisabelAudioListener(AudioListener listener)
    {
        listener.enabled = false;
    }
}
