using UnityEngine;
using System.Collections;


public class AnimAudioListener : MonoBehaviour
{
    public AudioClip footStep;
    public AudioClip simpleAttackAudio;
    public AudioClip jumpAttackAudio;

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

            case "simpleAttackAudio":
                PlayAudio(simpleAttackAudio);
                break;

            case "jumpAttackAudio":
                PlayAudio(jumpAttackAudio);
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
