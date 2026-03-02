using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class TopDownCharacterController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    private CharacterController _characterController;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        float horizontal = -UltimateJoystick.GetHorizontalAxis("Movement");
        float vertical = -UltimateJoystick.GetVerticalAxis("Movement");

        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical);

        // Clamp magnitude so diagonal movement isn't faster
        if (moveDirection.magnitude > 1f)
            moveDirection.Normalize();

        _characterController.Move(moveDirection * moveSpeed * Time.deltaTime);

        // Rotate character to face movement direction
        Vector3 lookDirection = new Vector3(horizontal, 0f, vertical);
        if (lookDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }
    }
}