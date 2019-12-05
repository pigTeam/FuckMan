using UnityEngine;
using System.Collections;
using Unity.Entities;

public class CustomInputSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        float hor = Input.GetAxis("Horizontal");
        bool isJump = Input.GetKeyDown(KeyCode.Space);
        bool atkTrigger = Input.GetAxis("Fire1") != 0;

        Entities.WithAll<MoveComponent>().WithAll<InputComponent>().ForEach((Entity id,ref MoveComponent moveData)=> {

            moveData.speed = hor * moveData.maxSpeed;
        });

        if(isJump)
        {
            Entities.WithAll<JumpComponent>().WithAll<InputComponent>().ForEach((Entity id, ref JumpComponent jumpData) => {
                jumpData.jumpTrigger = isJump;
            });
        }
       
        if(atkTrigger)
        {
            Entities.WithAll<InputComponent>()
                .WithAll<SimpleAttackComponent>().ForEach((Entity id, ref SimpleAttackComponent atkData) => {
                    atkData.attackTrigger = true;
            });
        }
    }
}
