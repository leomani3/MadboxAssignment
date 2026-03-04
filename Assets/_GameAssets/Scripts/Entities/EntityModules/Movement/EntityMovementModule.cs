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
    
    private bool m_isMoving;
    private bool m_canMove = true;

    public bool CanMove
    {
        get => m_canMove;
        set => m_canMove = value;
    }

    private void Update()
    {
        if(!m_canMove) return;
        
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

        m_characterController.Move(moveDirection * Owner.EntityData.moveSpeed * Time.deltaTime);
        m_animator.SetFloat("Speed", moveDirection.magnitude * Owner.EntityData.moveSpeed);
        
        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveDirection);
        }
    }
}