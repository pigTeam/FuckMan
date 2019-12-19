using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG;
using DG.Tweening;
using System;
using UniRx;

public class KnightSlash : MonoBehaviour
{
    public float maxDist = 1f;
    public float animTime = 0.2f;
    private SpriteRenderer spriteRenderer;

    // Use this for initialization
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Act(Action callback)
    {
        Vector3 endPosition = transform.position + transform.right * maxDist;
        transform.DOMove(endPosition, animTime);
        DOTween.ToAlpha(() => spriteRenderer.color, x => spriteRenderer.color = x, 0, animTime);

        Observable.Timer(TimeSpan.FromSeconds(0.2f))
            .Subscribe(x =>
            {
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
                if (callback != null)
                {
                    callback.Invoke();
                }
            });
    }

}
