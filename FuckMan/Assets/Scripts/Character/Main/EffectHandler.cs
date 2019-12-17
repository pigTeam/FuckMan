using UnityEngine;
using System.Collections;
using UniRx;
using System;

public class EffectHandler : MonoBehaviour
{
    public GameObject landEffectPf;
    public GameObject runEffectPf;
    public Transform landEffectPosition;
    public Transform runEffectPosition;

    private GameObject landEffect;
    private ParticleSystem runEffect;
    private Rigidbody2D rigidbody;
    private float velosityVertical = 0;
    // Use this for initialization
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
        if(rigidbody.velocity.y == 0)
        {
            if(velosityVertical < 0)
            {
                PlayLandEffect();
            }
        }
        velosityVertical = rigidbody.velocity.y;
    }

    void PlayLandEffect()
    {
        if(landEffect == null)
        {
            landEffect = Instantiate<GameObject>(landEffectPf);
        }

        landEffect.transform.position = landEffectPosition.position;
        landEffect.SetActive(true);
        Observable.Timer(TimeSpan.FromSeconds(1))
            .Subscribe(x => { landEffect.SetActive(false); });
    }

    public void OnMoveSpeed(float speed)
    {
        if (runEffect == null)
        {
            GameObject ego = Instantiate<GameObject>(runEffectPf);
            ego.transform.parent = runEffectPosition;
            ego.transform.localScale = Vector3.one;
            ego.transform.localPosition = Vector3.zero;
            runEffect = ego.GetComponent<ParticleSystem>();
        }
        var emission = runEffect.emission;

        if (speed!=0)
        {
            emission.enabled = true;
            Quaternion q = Quaternion.Euler(0, 0, 0);
            Vector3 position = Vector3.zero;
            if (speed < 0)
            {
                q = Quaternion.Euler(0, 0, 180);
                position = new Vector3(3.6f, 0, 0);
            }

            runEffect.transform.localPosition  = position;
            runEffect.transform.localRotation = q;
        }
        else
        {
            emission.enabled = false;

            // runEffect.Stop(true,ParticleSystemStopBehavior.StopEmitting);
        }
    }
}
