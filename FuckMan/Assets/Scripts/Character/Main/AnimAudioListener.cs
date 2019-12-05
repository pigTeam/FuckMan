using UnityEngine;
using System.Collections;


public class AnimAudioListener : MonoBehaviour
{
    public AudioClip footStep;
    public AudioClip simpleAttackAudio;
    public AudioClip jumpAttackAudio;
    public AudioClip landAudio;

    private AudioSource audioSource;
    private Rigidbody2D rigidBody;
    private float velosityVertical = 0;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if(audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if(rigidBody.velocity.y == 0 && velosityVertical < 0)
        {
            PlayAudio(landAudio);
        }
        velosityVertical = rigidBody.velocity.y;
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
