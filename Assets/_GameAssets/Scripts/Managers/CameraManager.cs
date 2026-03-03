using System;
using Cinemachine;
using MyBox;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    [SerializeField] private Camera mainCam;
    [SerializeField] private CinemachineVirtualCamera m_vCam;
    
    public Camera MainCam => mainCam;

    private void Awake()
    {
        InitializeSingleton();
    }

    public void Setup(Transform targetTfm, BoxCollider boundingBox)
    {
        m_vCam.Follow = targetTfm;
        if (m_vCam.TryGetComponent(out CinemachineConfiner confiner))
        {
            confiner.m_BoundingVolume = boundingBox;
        }
    }
}