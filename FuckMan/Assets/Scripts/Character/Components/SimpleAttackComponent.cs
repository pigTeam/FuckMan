using UnityEngine;
using System.Collections;
using Unity.Entities;

public struct SimpleAttackComponent : IComponentData
{
    public bool attackTrigger;
}
