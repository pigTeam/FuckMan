using UnityEngine;
using System.Collections;
using Unity.Entities;
using UniRx;

[UpdateBefore(typeof(JumpSystem))]
[UpdateAfter(typeof(CustomInputSystem))]
public class AnimationSystem : ComponentSystem
{
    protected override void OnUpdate()
    {

        Entities.WithAllReadOnly<AnimationComponent>()
            .ForEach((Entity id) => {

                //setup listener
                if(!AnimationEventListener.HasListenerSetup(id))
                {
                    AnimationEventListener.SetUp(id, OnAnimEvent);
                }

                Rigidbody2D rigidbody2D = EntityUtility.Instance.GetComponent<Rigidbody2D>(id);
                if (rigidbody2D != null)
                {
                    Animator animator = EntityUtility.Instance.GetComponent<Animator>(id);
                    if (animator != null)
                    {
                        animator.SetBool("IsLand", rigidbody2D.velocity.y == 0);
                    }
                }

            });

        Entities.WithAllReadOnly<AnimationComponent>()
            .WithAllReadOnly<MoveComponent>()
            .ForEach((Entity id, ref MoveComponent moveData) => {
           
            Animator animator = EntityUtility.Instance.GetComponent<Animator>(id);
            if(animator != null)
            {
                animator.SetBool("IsRun", moveData.speed != 0);
            }
        });

        Entities.WithAllReadOnly<AnimationComponent>()
            .WithAllReadOnly<JumpComponent>()
            .ForEach((Entity id, ref JumpComponent jumpData) => { 
            if(jumpData.jumpTrigger)
            {
                Animator animator = EntityUtility.Instance.GetComponent<Animator>(id);
                if (animator != null)
                {
                    animator.SetTrigger("Jump");
                }
            }
        });


        Entities.WithAllReadOnly<AnimationComponent>().
            WithAllReadOnly<SimpleAttackComponent>().
            ForEach((Entity id, ref SimpleAttackComponent atkData) => {
                if(atkData.attackTrigger)
                {
                    Animator animator = EntityUtility.Instance.GetComponent<Animator>(id);
                    if (animator != null)
                    {
                        animator.SetTrigger("AttackTrigger");
                        Observable.TimerFrame(1)
                        .Subscribe(x => { animator.ResetTrigger("AttackTrigger"); });
                    }
                }
            });
    }


    void OnAnimEvent(Entity id,AnimEvent animType)
    {
        switch (animType)
        {
            case AnimEvent.SimpleAttackStart:
                EntityUtility.Instance.RemoveAndPushEntityComponentData<MoveComponent>(id);
                EntityUtility.Instance.RemoveAndPushEntityComponentData<JumpComponent>(id);
                break;
            case AnimEvent.SimpleAttackEnd:
                EntityUtility.Instance.PopAndAddEntityComponentData<MoveComponent>(id);
                EntityUtility.Instance.PopAndAddEntityComponentData<JumpComponent>(id);
                break;
            default:
                break;
        }
    }
}
