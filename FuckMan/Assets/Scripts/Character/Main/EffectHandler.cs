using UnityEngine;
using System.Collections;
using UniRx;
using System;

public class EffectHandler : MonoBehaviour
{
    public GameObject landEffectPf;
    public Transform landEffectPosition;

    private GameObject landEffect;
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
}
