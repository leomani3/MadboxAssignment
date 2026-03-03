using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SkeletonMovementModule : EntityMovementModule
{ 
    protected override void HandleMovement()
    {
        base.HandleMovement();

        if (EntityManager.Instance.Player != null)
        {
            Vector3 moveDirection = (EntityManager.Instance.Player.transform.position - transform.position).normalized;
            Move(moveDirection);
        }
    }
}