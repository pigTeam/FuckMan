using UnityEngine;
using System.Collections;


public class AnimAudioListener : MonoBehaviour
{
    public AudioClip footStep;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if(audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void OnAnimAudioEvent(string name)
    {
        switch (name)
        {
            case "footStep":
                PlayAudio(footStep);
                break;

            default:
                break;
        }
    }

    private void PlayAudio(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
