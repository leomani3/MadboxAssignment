using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Transform m_targetTransform;
    [SerializeField] private BoxCollider m_cameraBoundingBox;

    private void Awake()
    {
        CameraManager.Instance.Setup(m_targetTransform, m_cameraBoundingBox);
    }
}