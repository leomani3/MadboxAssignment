using MyBox;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

public class PlayerAttackModule : EntityAttackModule
{
    [Header("Animation")]
    [SerializeField] private Animator m_animator;
    [SerializeField, ReadOnly] private float m_baseAnimationLength;
    [SerializeField] private ProjectilePoolRef m_projectilePoolRef;
    [SerializeField] private Transform m_projectileSpawnPoint;

    public void SetCanAttack(bool _canAttack)
    {
        m_canAttack = _canAttack;

        if (!m_canAttack)
            CancelAttack();
    }

    protected override void OnInitialize()
    {
        base.OnInitialize();
        UpdateAnimatorSpeed();
    }

    protected override void Update()
    {
        base.Update();     // ticks cooldown + finds target
        TryAttack();
    }

    // Called by the base when conditions are met (cooldown elapsed, target in range, etc.)
    protected override void PerformAttack()
    {
        base.PerformAttack();   // stamps the cooldown timer
        StartAttackAnimation();
    }

    private void StartAttackAnimation()
    {
        m_animator?.SetBool("Attack", true);

        // Face the target before swinging.
        if (m_currentTarget != null)
            transform.LookAt(m_currentTarget.transform.position.SetY(transform.position.y));
    }

    private void CancelAttack()
    {
        m_animator?.SetBool("Attack", false);
        m_currentTarget = null;
    }

    // Called by an Animation Event at the hit frame of the attack clip.
    public void OnAttackPerformed()
    {
        if (!m_canAttack)
        {
            CancelAttack();
            return;
        }
        
        Projectile.Launch(m_projectilePoolRef, m_projectileSpawnPoint.position, m_projectileSpawnPoint.rotation);
        
        m_animator?.SetBool("Attack", false);

        if (m_currentTarget == null) return;

        DealDamage(m_currentTarget, attackDamage);
    }

    // Scales the animator so the Attack clip always plays in exactly (1 / attackSpeed) seconds.
    private void UpdateAnimatorSpeed()
    {
        if (m_animator == null || m_baseAnimationLength <= 0f) return;

        float targetDuration = AttackInterval;
        m_animator.speed = m_baseAnimationLength / targetDuration;
    }

#if UNITY_EDITOR
    private void Reset()
    {
        m_animator = GetComponentInChildren<Animator>();
        BakeAttackClipLength();
    }

    private void OnValidate()
    {
        if (m_animator == null)
            m_animator = GetComponentInChildren<Animator>();

        BakeAttackClipLength();
    }

    private void BakeAttackClipLength()
    {
        if (m_animator == null) return;

        var controller = m_animator.runtimeAnimatorController as AnimatorController;
        if (controller == null) return;

        foreach (var layer in controller.layers)
        {
            float length = FindClipLengthInStateMachine(layer.stateMachine, "Attack");
            if (length > 0f)
            {
                m_baseAnimationLength = length;
                return;
            }
        }

        Debug.LogWarning($"[PlayerAttackModule] No state named 'Attack' found in the AnimatorController on '{name}'.");
    }

    private float FindClipLengthInStateMachine(AnimatorStateMachine stateMachine, string stateName)
    {
        foreach (var childState in stateMachine.states)
        {
            if (childState.state.name == stateName)
            {
                var clip = childState.state.motion as AnimationClip;
                return clip != null ? clip.length : 0f;
            }
        }

        foreach (var subStateMachine in stateMachine.stateMachines)
        {
            float length = FindClipLengthInStateMachine(subStateMachine.stateMachine, stateName);
            if (length > 0f) return length;
        }

        return 0f;
    }
#endif
}