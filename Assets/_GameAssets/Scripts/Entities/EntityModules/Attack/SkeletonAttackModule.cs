public class SkeletonAttackModule : EntityAttackModule
{
    protected override void Update()
    {
        base.Update();
        TryAttack();
    }

    protected override void PerformAttack()
    {
        base.PerformAttack();
        DealDamage(m_currentTarget, Owner.EntityData.attackDamage);
    }
}