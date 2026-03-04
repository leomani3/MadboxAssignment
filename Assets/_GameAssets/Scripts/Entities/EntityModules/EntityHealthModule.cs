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
    [SerializeField] private HealthBarPoolRef healthBarPoolRef;
    [SerializeField] private Vector3 healthBarOffset = new Vector3(0f, 2f, 0f);

    [Header("Damage Feedback – Punch Scale")]
    [SerializeField] private Vector3 punchScale = new Vector3(0.3f, -0.2f, 0f);
    [SerializeField, Min(0f)] private float punchDuration = 0.35f;
    [SerializeField, Min(1)] private int punchVibrato = 6;
    [SerializeField, Range(0f, 1f)] private float punchElasticity = 0.5f;

    [Header("Damage Feedback – White Sheen")]
    [SerializeField, Min(0f)] private float sheenHoldDuration = 0.05f;
    [SerializeField, Min(0f)] private float sheenFadeDuration = 0.15f;

    private float m_currentHealth;
    private bool m_isDead;
    private HealthBar m_healthBar;

    private Renderer[] m_renderers;
    private Color[] m_originalColors;
    private Sequence m_sheenSequence;
    private Tween m_punchTween;
    
    private static readonly int s_emissionColorID = Shader.PropertyToID("_EmissionColor");

    public float CurrentHealth => m_currentHealth;
    public float MaxHealth => Owner.EntityData.maxHealth;
    public bool IsDead => m_isDead;
    public float HealthPercent => MaxHealth > 0f ? m_currentHealth / MaxHealth : 0f;

    protected override void OnInitialize()
    {
        m_currentHealth = MaxHealth;
        m_isDead = false;

        CacheRenderers();
        SpawnHealthBar();
    }
    

    private void CacheRenderers()
    {
        m_renderers = Owner.GetComponentsInChildren<Renderer>(includeInactive: true);
        m_originalColors = new Color[m_renderers.Length];

        for (int i = 0; i < m_renderers.Length; i++)
        {
            Material mat = m_renderers[i].material;
            
            mat.EnableKeyword("_EMISSION");

            m_originalColors[i] = mat.HasProperty(s_emissionColorID)
                ? mat.GetColor(s_emissionColorID)
                : Color.black;
        }
    }

    private void PlayDamageFeedback()
    {
        PlayPunchScale();
        PlayWhiteSheen();
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

    private void PlayWhiteSheen()
    {
        m_sheenSequence?.Kill(complete: false);
        m_sheenSequence = DOTween.Sequence().SetLink(Owner.gameObject);

        for (int i = 0; i < m_renderers.Length; i++)
        {
            if (m_renderers[i] == null) continue;

            Material mat = m_renderers[i].material;
            if (!mat.HasProperty(s_emissionColorID)) continue;

            Color original = m_originalColors[i];
            
            mat.SetColor(s_emissionColorID, Color.white);
            
            Tween fade = mat
                .DOColor(original, s_emissionColorID, sheenFadeDuration)
                .SetDelay(sheenHoldDuration)
                .SetEase(Ease.OutQuad);

            m_sheenSequence.Join(fade);
        }

        m_sheenSequence.Play();
    }

    private void SpawnHealthBar()
    {
        Transform canvasTransform = UIManager.Instance.GameCanvas.transform;
        m_healthBar = healthBarPoolRef.pool.Spawn(canvasTransform);
        
        Transform trackTarget = GetOffsetTrackingTransform();
        m_healthBar.Setup(trackTarget, m_currentHealth, MaxHealth);
    }

    private void DespawnHealthBar()
    {
        if (m_healthBar == null) return;
        healthBarPoolRef.pool.Despawn(m_healthBar);
        m_healthBar = null;
    }
    
    private Transform GetOffsetTrackingTransform()
    {
        const string anchorName = "_HealthBarAnchor";
        Transform existing = Owner.transform.Find(anchorName);
        if (existing != null) return existing;

        GameObject anchor = new GameObject(anchorName);
        anchor.transform.SetParent(Owner.transform, worldPositionStays: false);
        anchor.transform.localPosition = healthBarOffset;
        return anchor.transform;
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
        amountText += amount.ToString();
        
        FloatingTextManager.Instance.SpawnUIText(
            CameraManager.Instance.MainCam.WorldToScreenPoint(transform.position.OffsetY(4)),
            amountText,
            isCrit ? GameAssets.Instance.critTextConfig : m_floatingTextConfig);

        UpdateHealthBar();
        PlayDamageFeedback();

        OnDamageTaken?.Invoke(amount, m_currentHealth);
        OnHealthChanged?.Invoke(m_currentHealth, MaxHealth, delta);

        if (m_currentHealth <= 0f)
            StartDeathAnimation();
    }

    [Button]
    public void Heal(float amount)
    {
        if (m_isDead || amount <= 0f) return;

        float previous = m_currentHealth;
        m_currentHealth = Mathf.Min(MaxHealth, m_currentHealth + amount);
        float delta = m_currentHealth - previous;

        UpdateHealthBar();

        OnHealed?.Invoke(amount, m_currentHealth);
        OnHealthChanged?.Invoke(m_currentHealth, MaxHealth, delta);
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
        m_sheenSequence?.Kill(complete: false);

        m_deathFxPoolRef.pool.Spawn(transform.position, Quaternion.identity, m_deathFxPoolRef.pool.transform);
        
        OnDeath?.Invoke();
    }
}