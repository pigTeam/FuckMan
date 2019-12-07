using UnityEngine;
using System.Collections;
using Unity.Entities;
using System;

[Serializable]
public struct MoveComponent : IComponentData
{
    public float maxSpeed;
    public float speed;
}
