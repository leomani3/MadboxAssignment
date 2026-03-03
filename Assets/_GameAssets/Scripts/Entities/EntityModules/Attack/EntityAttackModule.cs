using Sirenix.OdinInspector;
using UnityEngine;

public class EntityAttackModule : EntityModule
{
    [Header("Attack Settings")]
    [SerializeField] protected float attackDamage = 10f;
    [SerializeField] protected float attackSpeed = 1f;   // attacks per second
    [SerializeField] protected float attackRange = 1f;

    [SerializeField, ReadOnly] protected Entity m_currentTarget;

    protected bool m_canAttack;
    protected float m_attackCooldownTimer;

    protected bool IsAttackReady => m_attackCooldownTimer <= 0f;
    protected float AttackInterval => attackSpeed > 0f ? 1f / attackSpeed : float.MaxValue;

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
        OnTargetChanged(m_currentTarget, target);
        m_currentTarget = target;
    }
    
    protected virtual void OnTargetChanged(Entity oldTarget, Entity newTarget)
    {
    }

    protected bool IsInRange(Entity target)
    {
        float distSqr = (target.transform.position - Owner.transform.position).sqrMagnitude;
        return distSqr <= attackRange * attackRange;
    }

    // Child classes call this in Update to attempt an attack.
    protected virtual void TryAttack()
    {
        if (!m_canAttack || !IsAttackReady || m_currentTarget == null)
            return;

        PerformAttack();
    }

    // Override to implement the actual attack logic.
    // Always call base.PerformAttack() to stamp the cooldown.
    protected virtual void PerformAttack()
    {
        m_attackCooldownTimer = AttackInterval;
    }

    public void DealDamage(Entity target, float damage)
    {
        if (target.TryGetModule(out EntityHealthModule healthModule))
            healthModule.TakeDamage(damage);
    }
}