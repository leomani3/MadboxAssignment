using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

public class EntityAttackModule : EntityModule
{
    [Header("Attack Settings")]
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackSpeed = 1f; // attacks per second

    [Header("Animation")]
    [SerializeField] private Animator m_animator;
    [SerializeField, ReadOnly] private float m_baseAnimationLength;

    private Entity m_currentTarget;
    private bool m_isAttacking;
    private bool m_canAttack;

    public void SetCanAttack(bool _canAttack)
    {
        m_canAttack = _canAttack;

        if (!m_canAttack && m_isAttacking)
            CancelAttack();
    }

    protected override void OnInitialize()
    {
        if (m_animator == null)
        {
            Debug.LogWarning($"[EntityAttackModule] No Animator found on '{Owner.name}' or its children.");
            return;
        }

        if (m_baseAnimationLength <= 0f)
            Debug.LogWarning($"[EntityAttackModule] Base animation length is 0 on '{Owner.name}'. Make sure an 'Attack' state exists in the Animator.");

        m_currentTarget = null;
        m_isAttacking = false;
        m_canAttack = false;

        UpdateAnimatorSpeed();
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

        Debug.LogWarning($"[EntityAttackModule] No state named 'Attack' found in the AnimatorController on '{name}'.");
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

    private void Update()
    {
        if (!m_canAttack || m_isAttacking) return;

        UpdateTarget();

        if (m_currentTarget != null)
        {
            transform.LookAt(m_currentTarget.transform);
            StartAttackAnimation();
        }
    }

    private void UpdateTarget()
    {
        Entity closest = FindClosestEnemy();

        if (closest == m_currentTarget) return;

        m_currentTarget = closest;
    }

    private Entity FindClosestEnemy()
    {
        Entity closest = null;
        float closestDistSqr = float.MaxValue;

        foreach (Entity entity in EntityManager.Instance.AllEntities)
        {
            if (entity == Owner) continue;

            float distSqr = (entity.transform.position - Owner.transform.position).sqrMagnitude;
            if (distSqr < closestDistSqr)
            {
                closestDistSqr = distSqr;
                closest = entity;
            }
        }

        return closest;
    }

    private void StartAttackAnimation()
    {
        m_isAttacking = true;
        m_animator?.SetBool("Attack", true);
    }

    private void CancelAttack()
    {
        m_animator?.SetBool("Attack", false);
        m_isAttacking = false;
        m_currentTarget = null;
    }

    public void OnAttackPerformed()
    {
        if (!m_canAttack)
        {
            CancelAttack();
            return;
        }

        m_animator?.SetBool("Attack", false);
        m_isAttacking = false;

        if (m_currentTarget == null) return;

        if (m_currentTarget.TryGetModule<EntityHealthModule>(out var health))
            health.TakeDamage(attackDamage);
    }

    private void UpdateAnimatorSpeed()
    {
        if (m_animator == null || m_baseAnimationLength <= 0f) return;

        float targetDuration = 1f / attackSpeed;
        m_animator.speed = m_baseAnimationLength / targetDuration;
    }
}