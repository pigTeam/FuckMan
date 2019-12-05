using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Movement{
    public float maxSpeed;
    public float speed;
}

public struct Jumpment
{
    public float jumpForce;

    public bool jumpTrigger;
}

public struct Attackment
{
    public bool attackTrigger;
}
public class FrameData
{
    public Movement move = new Movement();
    public Jumpment jump = new Jumpment();
    public Attackment simpleAttack = new Attackment();
}
