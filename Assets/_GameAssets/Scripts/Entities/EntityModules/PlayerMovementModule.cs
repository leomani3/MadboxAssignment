using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementModule : EntityMovementModule
{ 
    [SerializeField] private SpriteRenderer m_movementSpite;

    protected override void HandleMovement()
    {
        base.HandleMovement();
        
        float horizontal = UltimateJoystick.GetHorizontalAxis("Movement");
        float vertical = UltimateJoystick.GetVerticalAxis("Movement");

        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        if (Owner.TryGetModule(out EntityAttackModule attackModule))
        {
            attackModule.SetCanAttack(moveDirection == Vector3.zero);
        }
        
        m_movementSpite.transform.position = transform.position + (moveDirection * 1.5f);
        
        Move(moveDirection);
    }
}