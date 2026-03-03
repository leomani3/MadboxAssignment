using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EntityMovementModule : EntityModule
{ 
    public Action OnStartedMoving;
    public Action OnStoppedMoving;
    
    [Header("references")]
    [SerializeField] protected CharacterController m_characterController;
    [SerializeField] protected Animator m_animator;
    
    [Header("Settings")]
    [SerializeField] protected float m_moveSpeed = 5f;
    
    private bool m_isMoving;

    public bool IsMoving => m_isMoving;

    private void Update()
    {
        HandleMovement();
    }

    protected virtual void HandleMovement()
    {
    }

    protected void Move(Vector3 moveDirection)
    {
        bool wasMoving = m_isMoving;
        m_isMoving = moveDirection != Vector3.zero;

        if (!wasMoving && m_isMoving)
        {
            OnStartedMoving?.Invoke();
        }
        else if (wasMoving && !m_isMoving)
        {
            OnStoppedMoving?.Invoke();
        }

        m_characterController.Move(moveDirection * m_moveSpeed * Time.deltaTime);
        m_animator.SetFloat("Speed", moveDirection.magnitude * m_moveSpeed);
        
        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveDirection);
        }
    }
}