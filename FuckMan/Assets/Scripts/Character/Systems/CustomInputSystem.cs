using UnityEngine;
using System.Collections;
using Unity.Entities;
using Matchvs;
public class CustomInputSystem : ComponentSystem
{
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
                }

                if (atkTrigger)
                {
                    frame.simpleAttack.attackTrigger = true;
                    isDirty = true;
                } else
                {
                   // atkData.attackTrigger = false;
                }

                if (isJump)
                {
                    frame.jump.jumpTrigger = isJump;
                    isDirty = true;
                } else
                {
                   // jumpData.jumpTrigger = false;
                }
            }
        });


        if (isDirty)
        {
            Game.FrameTime = System.DateTime.Now.Ticks;
            MatchvsEngine.getInstance().sendFrameEvent(DataUtil.Serialize(frame));
        }
    }
}
