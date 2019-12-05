using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
public struct FrameJumpComponent : IComponentData
{
    public List<Jumpment> jumps;
}
