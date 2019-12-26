using UnityEngine;
using System.Collections;
using Matchvs;
using System.Collections.Generic;
using System;

public class MatchVSDataParser
{
    public void Parse(MsFrameData data)
    {
        if (data.frameItems.Length > 0)
        {
            for (int i = 0; i < data.frameItems.Length; i++)
            {
                var item = data.frameItems.GetValue(i);
                if (item != null)
                {
                    FrameDataNotify notify = (FrameDataNotify)item;

                    try
                    {
                        var frameData = DataUtil.Deserialize<FrameData>(notify.CpProto);
                        switch (frameData.dataType)
                        {
                            case DataType.Input:
                                ParseInputData(frameData, notify.SrcUid);
                                break;

                            case DataType.Damage:
                                ParseDamageData(frameData, notify.SrcUid);
                                break;

                            case DataType.PositionSync:
                                ParsePositionSyncData(frameData, notify.SrcUid);
                                break;

                            default:
                                break;
                        }

                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                    }
                }

            }

        }
    }

    private void ParseInputData(FrameData frameData,uint srcUid)
    {
        InputData inputData = (InputData)frameData.data;
        List<MoveComponent> moveList;
        if (GameNetWork.frameMoves.TryGetValue(srcUid, out moveList))
        {

            moveList.Add(inputData.move);

        }
        else
        {
            moveList = new List<MoveComponent>();
            GameNetWork.frameMoves.Add(srcUid, moveList);
        }



        List<JumpComponent> jumpList;
        if (GameNetWork.frameJumps.TryGetValue(srcUid, out jumpList))
        {

            jumpList.Add(inputData.jump);

        }
        else
        {
            jumpList = new List<JumpComponent>();
            GameNetWork.frameJumps.Add(srcUid, jumpList);
        }


        List<SimpleAttackComponent> attackList;
        if (GameNetWork.frameAttacks.TryGetValue(srcUid, out attackList))
        {

            attackList.Add(inputData.simpleAttack);

        }
        else
        {
            attackList = new List<SimpleAttackComponent>();
            GameNetWork.frameAttacks.Add(srcUid, attackList);
        }

        
    }

    private void ParseDamageData(FrameData frameData, uint srcUid)
    {
        DamageComponent damageData = (DamageComponent)frameData.data;
        GameNetWork.Inst.HandleDamage(damageData);
    }

    private void ParsePositionSyncData(FrameData frameData, uint srcUid)
    {
        PositionData positionData = (PositionData)frameData.data;

        Vector3 position = positionData.ToVector3();
        if (GameNetWork.framePositionChecks.ContainsKey(srcUid))
        {

            GameNetWork.framePositionChecks[srcUid] = position;
        }
        else
        {
            GameNetWork.framePositionChecks.Add(srcUid, position);
        }
    }
}
