using UnityEngine;
using System.Collections;
using Unity.Entities;
using System;

[Serializable]
public struct SimpleAttackComponent : IComponentData
{
    public bool attackTrigger;
}
