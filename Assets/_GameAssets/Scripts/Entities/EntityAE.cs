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
}