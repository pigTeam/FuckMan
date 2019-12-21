using UnityEngine;
using System.Collections;
using Unity.Entities;
using System;

[Serializable]
public struct DamageComponent : IComponentData
{
    public int value;
    public uint fromUserId;
    public uint targetUserId;
}
