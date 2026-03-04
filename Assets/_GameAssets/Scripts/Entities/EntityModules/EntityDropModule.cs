using System;
using UnityEngine;

public class EntityDropModule : EntityModule
{
    [SerializeField] private int m_nbOrbToDrop;
    [SerializeField] private XpOrbPoolRef m_xpOrbPoolRef;

    public override void OnAllModuleInitialized()
    {
        base.OnAllModuleInitialized();

        if (Owner.TryGetModule(out EntityHealthModule healthModule))
        {
            healthModule.OnDeathStart += OnEntityDeathStart;
        }
    }

    private void OnDisable()
    {
        if (Owner.TryGetModule(out EntityHealthModule healthModule))
        {
            healthModule.OnDeathStart -= OnEntityDeathStart;
        }
    }

    private void OnEntityDeathStart()
    {
        for (int i = 0; i < m_nbOrbToDrop; i++)
        {
            XpOrb orb = m_xpOrbPoolRef.pool.Spawn(transform.position, Quaternion.identity, m_xpOrbPoolRef.pool.transform);
            orb.Setup(m_xpOrbPoolRef.pool);
        }
    }
}