using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
public struct FrameAttackComponent : IComponentData
{
    public List<Attackment> attacks;
}
