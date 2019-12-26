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

    private Action<GameObject,Vector2> onSlashDead;
    private IDisposable deadTask;
    private CharacterBase _owner;
    private Tweener tweener;

    private GameObject slashHit;

    // Use this for initialization
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        slashHit = Resources.Load<GameObject>("slashHit");
        slashHit = GameObject.Instantiate(slashHit);
        slashHit.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Act(CharacterBase owner,Action<GameObject,Vector2> callback)
    {
        _owner = owner;
        onSlashDead += callback;
        Vector3 endPosition = transform.position + transform.right * maxDist;
        transform.DOMove(endPosition, animTime);
        tweener = DOTween.ToAlpha(() => spriteRenderer.color, x => spriteRenderer.color = x, 0, animTime);

        deadTask = Observable.Timer(TimeSpan.FromSeconds(0.2f))
            .Subscribe(x =>
            {
                TriggerSlashDead(null,Vector2.zero);
            });
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && _owner != null && collision.gameObject != _owner.gameObject)
        {
            ShowSlashHit();
            TriggerSlashDead(collision.gameObject,transform.right.normalized);
        }
    }

    private void TriggerSlashDead(GameObject target,Vector2 dir)
    {
        if(tweener != null)
        {
            tweener.Kill();
        }
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
        _owner = null;
        if (deadTask != null)
        {
            deadTask.Dispose();
            deadTask = null;
        }

        if (onSlashDead != null)
        {
            onSlashDead.Invoke(target,dir);
            onSlashDead = null;
        }
    }

    private void ShowSlashHit()
    {
        slashHit.transform.position = transform.position+ transform.right * 0.35f;
        slashHit.SetActive(true);
        Observable.TimerFrame(3)
            .Subscribe(x => { slashHit.SetActive(false); });
    }
}
