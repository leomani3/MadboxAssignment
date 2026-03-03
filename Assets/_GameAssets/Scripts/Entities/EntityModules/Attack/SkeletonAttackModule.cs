using UnityEngine;

public class SkeletonAttackModule : EntityAttackModule
{
    protected override void Update()
    {
        base.Update();     // ticks cooldown + finds target
        TryAttack();
        print(m_canAttack + " | " + IsAttackReady + " | " + m_currentTarget);
    }

    protected override void PerformAttack()
    {
        print("PerformAttack");
        base.PerformAttack();   // stamps the cooldown timer
        DealDamage(m_currentTarget, attackDamage);
    }
}