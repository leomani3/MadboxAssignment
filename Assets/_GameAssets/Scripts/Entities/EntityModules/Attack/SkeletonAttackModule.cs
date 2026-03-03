using UnityEngine;

public class SkeletonAttackModule : EntityAttackModule
{
    protected override void Update()
    {
        base.Update();     // ticks cooldown + finds target
        TryAttack();
    }

    protected override void PerformAttack()
    {
        base.PerformAttack();   // stamps the cooldown timer
        DealDamage(m_currentTarget, attackDamage);
    }
}