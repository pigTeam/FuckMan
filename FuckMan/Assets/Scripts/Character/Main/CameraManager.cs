using UnityEngine;
using System.Collections;
using Cinemachine;

public class CameraManager : MonoSingleton<CameraManager>
{
    public CinemachineVirtualCamera virtualCamera;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LockFollowTarget(Transform target)
    {
        virtualCamera.Follow = target;
    }
}
