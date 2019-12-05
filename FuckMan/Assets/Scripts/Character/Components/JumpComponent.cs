using UnityEngine;
using System.Collections;
using Unity.Entities;

public struct JumpComponent : IComponentData
{
    public float jumpForce;

    public bool jumpTrigger;
}
