using UnityEngine;
using System.Collections;
using Unity.Entities;
using System;

[Serializable]
public struct JumpComponent : IComponentData
{
    public float jumpForce;

    public bool jumpTrigger;
}
