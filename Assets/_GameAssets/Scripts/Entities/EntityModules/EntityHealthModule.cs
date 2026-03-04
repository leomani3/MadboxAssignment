using System;
using MyBox;
using Sirenix.OdinInspector;
using UnityEngine;
using DG.Tweening;

public class EntityHealthModule : EntityModule
{
    public Action<float, float, float> OnHealthChanged;
    public Action<float, float> OnDamageTaken;
    public Action<float, float> OnHealed;
    public Action OnDeathStart;
    public Action OnDeath;
    
    [Header("References")]
    [SerializeField] private Animator m_animator;
    [SerializeField] private ParticleSystemPoolRef m_deathFxPoolRef;
    [SerializeField] private FloatingTextConfig m_floatingTextConfig;

    [Header("Health Bar")]
    [SerializeField] private HealthBarPoolRef m_healthBarPoolRef;
    [SerializeField] private Transform m_healthBarTarget;

    [Header("Damage Feedback – Punch Scale")]
    [SerializeField] private Vector3 punchScale = new Vector3(0.3f, -0.2f, 0f);
    [SerializeField, Min(0f)] private float punchDuration = 0.35f;
    [SerializeField, Min(1)] private int punchVibrato = 6;
    [SerializeField, Range(0f, 1f)] private float punchElasticity = 0.5f;

    private float m_currentHealth;
    private bool m_isDead;
    private HealthBar m_healthBar;
    private Tween m_punchTween;
    
    public float MaxHealth => Owner.EntityData.maxHealth;

    protected override void OnInitialize()
    {
        m_currentHealth = MaxHealth;
        m_isDead = false;
        SpawnHealthBar();
    }
    
    private void PlayDamageFeedback()
    {
        PlayPunchScale();

        if (Owner.TryGetModule(out EntitySheenModule sheenModule))
        {
            sheenModule.PlayWhiteSheen();
        }
    }

    private void PlayPunchScale()
    {
        m_punchTween?.Kill(complete: true);
        Owner.transform.localScale = Vector3.one;

        m_punchTween = Owner.transform
            .DOPunchScale(punchScale, punchDuration, punchVibrato, punchElasticity)
            .SetUpdate(UpdateType.Normal)
            .SetLink(Owner.gameObject);
    }

    private void SpawnHealthBar()
    {
        Transform canvasTransform = UIManager.Instance.GameCanvas.transform;
        m_healthBar = m_healthBarPoolRef.pool.Spawn(canvasTransform);
        
        m_healthBar.Setup(m_healthBarTarget, m_currentHealth, MaxHealth);
    }

    private void DespawnHealthBar()
    {
        if (m_healthBar == null) return;
        m_healthBarPoolRef.pool.Despawn(m_healthBar);
        m_healthBar = null;
    }

    private void UpdateHealthBar()
    {
        if (m_healthBar == null) return;
        m_healthBar.SetValue(m_currentHealth, MaxHealth);
    }

    [Button]
    public void TakeDamage(float amount, bool isCrit)
    {
        if (m_isDead || amount <= 0f) return;

        float previous = m_currentHealth;
        m_currentHealth = Mathf.Max(0f, m_currentHealth - amount);
        float delta = m_currentHealth - previous;

         string amountText = "";
         if (isCrit)
         {
             amountText += "<sprite=\"crit\" name=\"crit\"> ";
         }
        amountText += amount.ToString("N0");
        
        FloatingTextManager.Instance.SpawnUIText(
            CameraManager.Instance.MainCam.WorldToScreenPoint(m_healthBarTarget.position) + new Vector3(0, 50, 0),
            amountText,
            isCrit ? GameAssets.Instance.critTextConfig : m_floatingTextConfig);

        UpdateHealthBar();
        PlayDamageFeedback();

        OnDamageTaken?.Invoke(amount, m_currentHealth);
        OnHealthChanged?.Invoke(m_currentHealth, MaxHealth, delta);

        if (m_currentHealth <= 0f)
            StartDeathAnimation();
    }

    private void StartDeathAnimation()
    {
        if (m_isDead) return;
        m_isDead = true;
        
        OnDeathStart?.Invoke();

        if (m_healthBar != null)
            m_healthBar.HideGauge(true, () => DespawnHealthBar());

        if (m_animator != null)
        {
            m_animator.SetTrigger("Die");
        }
    }

    public void Die()
    {
        m_punchTween?.Kill(complete: true);

        m_deathFxPoolRef.pool.Spawn(transform.position, Quaternion.identity, m_deathFxPoolRef.pool.transform);
        
        OnDeath?.Invoke();
    }
}