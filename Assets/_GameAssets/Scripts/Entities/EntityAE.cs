using UnityEngine;

public class EntityAE : MonoBehaviour
{
    private Entity m_entity;

    private void Awake()
    {
        m_entity = GetComponentInParent<Entity>();
    }

    public void OnAttackPerformed()
    {
        if (m_entity.TryGetModule(out PlayerAttackModule attackModule))
        {
            attackModule.OnAttackPerformed();
        }
    }

    public void OnSpawnAnimationStart()
    {
        m_entity.CanBeTargeted = false;
        if (m_entity.TryGetModule(out EntityMovementModule movementModule))
        {
            movementModule.CanMove = false;
        }
        
        if (m_entity.TryGetModule(out EntityAttackModule attackModule))
        {
            attackModule.SetCanAttack(false);
        }
    }

    public void OnSpawnAnimationEnd()
    {
        m_entity.CanBeTargeted = true;
        if (m_entity.TryGetModule(out EntityMovementModule movementModule))
        {
            movementModule.CanMove = true;
        }
        
        if (m_entity.TryGetModule(out EntityAttackModule attackModule))
        {
            attackModule.SetCanAttack(true);
        }
    }
}