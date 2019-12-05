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
        FrameData frame = new FrameData();
        bool isDirty = false;
        Entities.WithAll<MoveComponent>().WithAll<InputComponent>().ForEach((Entity id,ref MoveComponent moveData)=> {

            //moveData.speed = hor * moveData.maxSpeed;
            frame.move.speed = hor * moveData.maxSpeed;
            isDirty = true;
        });

        if(isJump)
        {
            Entities.WithAll<JumpComponent>().WithAll<InputComponent>().ForEach((Entity id, ref JumpComponent jumpData) => {
                frame.jump.jumpTrigger = isJump;
                isDirty = true;
            });
        }
       
        if(atkTrigger)
        {
            Entities.WithAll<InputComponent>()
                .WithAll<SimpleAttackComponent>().ForEach((Entity id, ref SimpleAttackComponent atkData) => {
                    frame.simpleAttack.attackTrigger = true;
                    isDirty = true;
                });
        }

        if (isDirty)
        {
            Game.FrameTime = System.DateTime.Now.Ticks;
            MatchvsEngine.getInstance().sendFrameEvent(DataUtil.Serialize(frame));
        }
    }
}
