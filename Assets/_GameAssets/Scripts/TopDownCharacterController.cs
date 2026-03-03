using MyBox;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(CharacterController))]
public class TopDownCharacterController : MonoBehaviour
{ 
    [Header("references")]
    [SerializeField] private CharacterController m_characterController;
    [SerializeField] private Animator m_animator;
    [SerializeField] private SpriteRenderer m_movementSpite;
    
    [Header("Settings")]
    [SerializeField] private float m_moveSpeed = 5f;

    [ButtonMethod]
    private void FindReferences()
    {
        m_characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        float horizontal = UltimateJoystick.GetHorizontalAxis("Movement");
        float vertical = UltimateJoystick.GetVerticalAxis("Movement");

        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;
        
        m_characterController.Move(moveDirection * m_moveSpeed * Time.deltaTime);
        
        m_animator.SetFloat("Speed", moveDirection.magnitude * m_moveSpeed);
        
        Vector3 lookDirection = new Vector3(horizontal, 0f, vertical);
        if (lookDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(lookDirection);
            m_movementSpite.transform.position = transform.position + (lookDirection * 1.5f);
        }
    }
}