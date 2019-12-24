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

        Entities.WithAll<CustomInputComponent, MoveComponent>().ForEach((Entity id, ref MoveComponent moveData) =>
        {
            moveData.speed = hor * moveData.maxSpeed;
        });

        Entities.WithAll<CustomInputComponent, SimpleAttackComponent>().ForEach((Entity id, ref SimpleAttackComponent attack) =>
        {
            attack.attackTrigger = atkTrigger;
        });

        Entities.WithAll<CustomInputComponent, JumpComponent>().ForEach((Entity id, ref JumpComponent jump) =>
        {
            jump.jumpTrigger = isJump;
        });
    }
}
