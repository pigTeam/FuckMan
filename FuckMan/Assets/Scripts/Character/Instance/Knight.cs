using UnityEngine;
using System.Collections;

public class Knight : CharacterBase
{
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigidbody;
    private KnightSlashPool slashHandler;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();
        slashHandler = GetComponentInChildren<KnightSlashPool>();
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            if(_isSelf)
            {
                var damage = new DamageComponent() { fromUserId = 0, targetUserId = userID, value = simpleAttackDamage };
                SetComponentData<DamageComponent>(damage);
            }
        }
    }

    public override void HandleAnimEvent(AnimEvent eventType)
    {
        base.HandleAnimEvent(eventType);
        switch (eventType)
        {
            case AnimEvent.SimpleAttackStart:
                break;
            case AnimEvent.SimpleAttackEnd:
                break;
            case AnimEvent.SimpleAttackEffect:
                HandleSlashEffect();
                break;
            default:
                break;
        }
    }

    private void HandleSlashEffect()
    {
        Direction dir = Direction.Right;
        if(spriteRenderer.flipX)
        {
            dir = Direction.Left;
        }

        if(rigidbody.velocity.y != 0 && Input.GetAxis("Vertical") < 0)
        {
            dir = Direction.Down;
        }
        slashHandler.GenerateSlash(this, dir);
    }


}
