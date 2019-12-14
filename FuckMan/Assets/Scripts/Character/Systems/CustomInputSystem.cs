using UnityEngine;
using System.Collections;
using Unity.Entities;
using Matchvs;
public class CustomInputSystem : ComponentSystem
{
    private bool isMove = false;
    private bool isAttack = false;
    private bool isJumpa = false;
    protected override void OnUpdate()
    {
        float hor = Input.GetAxis("Horizontal");
        bool isJump = Input.GetKeyDown(KeyCode.Space);
        bool atkTrigger = Input.GetAxis("Fire1") != 0;
        FrameData frame = new FrameData() { move = new MoveComponent(), jump = new JumpComponent(), simpleAttack = new SimpleAttackComponent()};
        bool isDirty = false;

        Entities.WithAll<MoveComponent, UserDataComponent, JumpComponent, SimpleAttackComponent>().WithAll<InputComponent>().ForEach((Entity id, ref MoveComponent moveData, ref UserDataComponent userData, ref JumpComponent jumpData, ref SimpleAttackComponent atkData) => {

           
          if (userData.isSelf)
            {
                if (hor != 0)
                {
                    frame.move.speed = hor * moveData.maxSpeed;
                    isDirty = true;
                    isMove = true;
                } else
                {
                    if (isMove)
                    {
                        frame.move.speed = hor * moveData.maxSpeed;
                        isDirty = true;
                    }
                    isMove = false;
                }

                if (atkTrigger)
                {
                    frame.simpleAttack.attackTrigger = true;
                    isDirty = true;
                    isAttack = true;
                } else
                {
                    if (isAttack)
                    {
                        frame.simpleAttack.attackTrigger = false;
                        isDirty = true;
                    }
                    isAttack = false;
                }

                if (isJump)
                {
                    frame.jump.jumpTrigger = isJump;
                    frame.jump.jumpForce = jumpData.jumpForce;
                    isDirty = true;
                    isJumpa = true;
                } else
                {
                    if (isJumpa)
                    {
                        frame.jump.jumpTrigger = false;
                        isDirty = true;
                    }
                    isJumpa = false;
                }
            }
        });


        if (isDirty)
        {
            GameNetWork.FrameTime = System.DateTime.Now.Ticks;
            MatchvsEngine.getInstance().sendFrameEvent(DataUtil.Serialize(frame));
        }
    }
}
