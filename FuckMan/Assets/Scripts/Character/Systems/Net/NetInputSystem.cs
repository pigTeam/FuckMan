﻿using UnityEngine;
using System.Collections;
using Unity.Entities;
using Matchvs;

public class NetInputSystem : ComponentSystem
{
    private bool isMove = false;
    private bool isAttack = false;
    private bool isJumpa = false;

    protected override void OnUpdate()
    {
        float hor = Input.GetAxis("Horizontal");
        bool isJump = Input.GetKeyDown(KeyCode.Space);
        bool atkTrigger = Input.GetAxis("Fire1") != 0;
        InputData inputData = new InputData() { move = new MoveComponent(), jump = new JumpComponent(), simpleAttack = new SimpleAttackComponent() };
        bool isDirty = false;

        Entities.WithAll<MoveComponent, UserDataComponent, JumpComponent, SimpleAttackComponent>()
            .WithAll<NetInputComponent>()
            .ForEach((Entity id, ref MoveComponent moveData,
            ref UserDataComponent userData, ref JumpComponent jumpData,
            ref SimpleAttackComponent atkData) => {

            if (userData.isSelf)
            {
                Transform trans = EntityUtility.Instance.GetComponent<Transform>(id);
                if (trans != null)
                {
                    inputData.position = new Vect3(trans.position);
                }

                if (hor != 0)
                {
                    inputData.move.speed = hor * moveData.maxSpeed;
                    isDirty = true;
                    isMove = true;
                }
                else
                {
                    if (isMove)
                    {
                        inputData.move.speed = hor * moveData.maxSpeed;
                        isDirty = true;
                    }
                    isMove = false;
                }

                if (atkTrigger)
                {
                    inputData.simpleAttack.attackTrigger = true;
                    isDirty = true;
                    isAttack = true;
                }
                else
                {
                    if (isAttack)
                    {
                        inputData.simpleAttack.attackTrigger = false;
                        isDirty = true;
                    }
                    isAttack = false;
                }

                if (isJump)
                {
                    inputData.jump.jumpTrigger = isJump;
                    inputData.jump.jumpForce = jumpData.jumpForce;
                    isDirty = true;
                    isJumpa = true;
                }
                else
                {
                    if (isJumpa)
                    {
                        inputData.jump.jumpTrigger = false;
                        isDirty = true;
                    }
                    isJumpa = false;
                }
            }
        });


        if (isDirty)
        {
            FrameData frame = new FrameData() { dataType = DataType.INPUT, data = inputData};
            GameNetWork.FrameTime = System.DateTime.Now.Ticks;
            MatchvsEngine.getInstance().sendFrameEvent(DataUtil.Serialize(frame));
        }
    }

}