using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
public struct FrameMoveComponent : IComponentData
{
    public List<Movement> moves;
}
