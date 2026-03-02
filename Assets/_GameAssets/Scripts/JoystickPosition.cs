using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickPosition : MonoBehaviour
{
    private Vector3 m_initialPosition;

    private void Awake()
    {
        m_initialPosition = transform.position;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            transform.position = Input.mousePosition;
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            transform.position = m_initialPosition;
        }
    }
}