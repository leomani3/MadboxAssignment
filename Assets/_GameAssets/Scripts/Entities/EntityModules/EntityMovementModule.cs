using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EntityMovementModule : EntityModule
{ 
    [Header("references")]
    [SerializeField] protected CharacterController m_characterController;
    [SerializeField] protected Animator m_animator;
    
    [Header("Settings")]
    [SerializeField] protected float m_moveSpeed = 5f;

    private void Update()
    {
        HandleMovement();
    }

    protected virtual void HandleMovement()
    {
    }

    protected void Move(Vector3 moveDirection)
    {
        m_characterController.Move(moveDirection * m_moveSpeed * Time.deltaTime);
        m_animator.SetFloat("Speed", moveDirection.magnitude * m_moveSpeed);
        
        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveDirection);
        }
    }
}