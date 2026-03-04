using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SkeletonMovementModule : EntityMovementModule
{ 
    protected override void HandleMovement()
    {
        base.HandleMovement();

        if (EntityManager.Instance.Player != null)
        {
            Vector3 vectorToPlayer = EntityManager.Instance.Player.transform.position - transform.position;
            vectorToPlayer.y = 0;
            Vector3 moveVector = vectorToPlayer.magnitude < 0.5f ? Vector3.zero : vectorToPlayer;
            Move(moveVector.normalized);
        }
    }
}