using Sirenix.OdinInspector;
using UnityEngine;

public class EntityAttackModule : EntityModule
{
    [SerializeField, ReadOnly] protected Entity m_currentTarget;

    protected bool m_canAttack;
    protected float m_attackCooldownTimer;

    private EntityData EntityData => Owner.EntityData;
    protected bool IsAttackReady => m_attackCooldownTimer <= 0f;
    protected float AttackInterval => EntityData.attackSpeed > 0f ? 1f / EntityData.attackSpeed : float.MaxValue;

    protected override void OnInitialize()
    {
        SetTarget(null);
        m_canAttack = true;
        m_attackCooldownTimer = 0f;
    }

    protected virtual void Update()
    {
        TickCooldown();
        FindTarget();
    }

    public virtual void SetCanAttack(bool _canAttack)
    {
        m_canAttack = _canAttack;
    }

    private void TickCooldown()
    {
        if (m_attackCooldownTimer > 0f)
            m_attackCooldownTimer -= Time.deltaTime;
    }

    private void FindTarget()
    {
        if (m_currentTarget != null && !IsInRange(m_currentTarget))
            SetTarget(null);

        Entity closest = EntityManager.Instance.FindClosestEnemy(Owner);

        if (closest != null && IsInRange(closest))
            SetTarget(closest);
    }
    
    protected void SetTarget(Entity target)
    {
        if (m_currentTarget == target) return;

        if (m_currentTarget != null && m_currentTarget.TryGetModule(out EntityHealthModule oldHealthModule))
            oldHealthModule.OnDeath -= OnTargetDied;

        OnTargetChanged(m_currentTarget, target);
        m_currentTarget = target;

        if (m_currentTarget != null && m_currentTarget.TryGetModule(out EntityHealthModule newHealthModule))
            newHealthModule.OnDeath += OnTargetDied;
    }

    protected virtual void OnTargetChanged(Entity oldTarget, Entity newTarget)
    {
    }

    private void OnTargetDied()
    {
        if (m_currentTarget != null && m_currentTarget.TryGetModule(out EntityHealthModule healthModule))
            healthModule.OnDeath -= OnTargetDied;

        m_currentTarget = null;
        FindTarget();
    }

    protected bool IsInRange(Entity target)
    {
        float distSqr = (target.transform.position - Owner.transform.position).sqrMagnitude;
        return distSqr <= EntityData.attackRange * EntityData.attackRange;
    }
    
    protected virtual void TryAttack()
    {
        if (!m_canAttack || !IsAttackReady || m_currentTarget == null)
            return;

        PerformAttack();
    }
    
    protected virtual void PerformAttack()
    {
        m_attackCooldownTimer = AttackInterval;
    }

    public void DealDamage(Entity target, float damage)
    {
        if (target == null) return;
        
        bool isCrit = Random.value > Owner.EntityData.critChance;

        if (isCrit)
        {
            damage *= 1.5f;
        }

        if (target.TryGetModule(out EntityHealthModule healthModule))
        {
            healthModule.TakeDamage(damage, isCrit);
        }
    }
}