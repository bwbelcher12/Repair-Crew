using UnityEngine;
using Mirror;
using UnityEngine.Audio;

public class PlayOneshot : NetworkBehaviour
{

    [SerializeField] AudioClip clip;
    [SerializeField] AudioMixerGroup output;
    [SerializeField] GameObject sourceObject;
    private AudioSource audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = sourceObject.GetComponent<AudioSource>();
    }

    public void PlayClip()
    {
        audioSource.PlayOneShot(clip, 1);
    }
}

