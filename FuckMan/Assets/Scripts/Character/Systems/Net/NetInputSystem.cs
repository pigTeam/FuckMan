using UnityEngine;
using System.Collections;
using Unity.Entities;
using Matchvs;

public class NetInputSystem : ComponentSystem
{
    private float syncPositionTimeSpan = 1;
    private float syncPositionTimer = 0;

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

        //SyncPosition
        syncPositionTimer += Time.deltaTime;
        if (syncPositionTimer > syncPositionTimeSpan)
        {
            Entities.WithAllReadOnly<UserDataComponent>().ForEach((Entity id, ref UserDataComponent userData) => {
                if(userData.isSelf)
                {
                    Transform trans = EntityUtility.Instance.GetComponent<Transform>(id);
                    if (trans != null)
                    {
                        PositionData positionData = new PositionData(trans.position);
                        FrameData frameData = new FrameData() { dataType = DataType.PositionSync, data = positionData };
                        //发送位置同步消息
                        GameNetWork.Inst.SendFrameData(frameData);
                    }
                }
            });
            
            syncPositionTimer = 0;
        }
        
        Entities.WithAll<MoveComponent, UserDataComponent, JumpComponent, SimpleAttackComponent>()
            .WithAll<NetInputComponent>()
            .ForEach((Entity id, ref MoveComponent moveData,
            ref UserDataComponent userData, ref JumpComponent jumpData,
            ref SimpleAttackComponent atkData) => {

            if (userData.isSelf)
            {
                

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
            FrameData frame = new FrameData() { dataType = DataType.Input, data = inputData};
            GameNetWork.FrameTime = System.DateTime.Now.Ticks;
            GameNetWork.Inst.SendFrameData(frame);
        }
    }

}
