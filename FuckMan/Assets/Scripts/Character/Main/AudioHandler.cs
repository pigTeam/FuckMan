using UnityEngine;
using System.Collections;


public class AudioHandler : MonoBehaviour
{
    public AudioClip footStep;
    public AudioClip simpleAttackAudio;
    public AudioClip jumpAttackAudio;
    public AudioClip landAudio;
    public AudioClip hitAudio;
    public AudioClip hitedAudio;

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
        bool case1 = Mathf.Approximately(rigidBody.velocity.y, 0) && velosityVertical < 0;
        bool case2 = rigidBody.velocity.y > 0 && velosityVertical < 0;
        if (case1 || case2)
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

    public void OnHitSomeBody()
    {
        if(hitAudio != null)
        {
            PlayAudio(hitAudio);
        }
    }

    public void OnGetHited()
    {
        if (hitedAudio != null)
        {
            PlayAudio(hitedAudio);
        }
    }
}
