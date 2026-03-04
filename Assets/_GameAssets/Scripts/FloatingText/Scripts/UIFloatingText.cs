using UnityEngine;
using DG.Tweening;
using Lean.Pool;
using MyBox;
using Sirenix.OdinInspector;
using TMPro;

public class UIFloatingText : MonoBehaviour, IPoolable
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Transform m_parent;
    
    [SerializeField] private FloatingTextConfig m_linkedConfig;
    private LeanUIFloatingTextPool m_linkedPool;
    
    // Stored tweens
    private Sequence _seq;
    private Tween _moveYTween;
    private Tween _moveXTween;

    [Button]
    public void Preview()
    {
        text.color = m_linkedConfig.color;
        text.font = m_linkedConfig.font;
        text.fontSize = m_linkedConfig.fontSize;

#if UNITY_EDITOR
        if (Application.isPlaying)
            Play();
#endif
    }

    private void Reset()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Init(string message, FloatingTextConfig config, LeanUIFloatingTextPool linkedPool)
    {
        text.text = message;
        m_linkedConfig = config;
        m_linkedPool = linkedPool;
        text.color = m_linkedConfig.color;
        text.font = m_linkedConfig.font;
        text.fontSize = m_linkedConfig.fontSize;
    }

    // IPoolable: called when the object is spawned from the pool
    public void OnSpawn()
    {
        Play();
    }

    // IPoolable: called when the object is despawned back into the pool
    public void OnDespawn()
    {
        KillAllTweens();
    }

    private void KillAllTweens()
    {
        if (_seq != null && _seq.IsActive())
        {
            _seq.Kill();
            _seq = null;
        }

        if (_moveYTween != null && _moveYTween.IsActive())
        {
            _moveYTween.Kill();
            _moveYTween = null;
        }

        if (_moveXTween != null && _moveXTween.IsActive())
        {
            _moveXTween.Kill();
            _moveXTween = null;
        }
    }

    public void Play()
    {
        text.color = text.color.WithAlphaSetTo(m_linkedConfig.enableFadeIn ? 0 : 1);
        m_parent.localScale = m_linkedConfig.enableScaleIn ? Vector3.zero : Vector3.one;
        m_parent.localPosition = Vector3.zero;

        KillAllTweens();

        float totalMoveDuration = m_linkedConfig.spawnDuration + m_linkedConfig.stayDuration;

        // ------- Movement tweens (stored separately, run outside the sequence)
        _moveYTween = m_parent
            .DOLocalMoveY(m_linkedConfig.YOffset, totalMoveDuration)
            .SetRelative();

        if (m_linkedConfig.randomXMovement)
        {
            _moveXTween = m_parent
                .DOLocalMoveX(Random.Range(m_linkedConfig.minMaxXOffset.x, m_linkedConfig.minMaxXOffset.y), totalMoveDuration)
                .SetRelative();
        }

        // ------- Sequence
        _seq = DOTween.Sequence();

        // Spawn: fade in
        _seq.Append(text.DOFade(1f, m_linkedConfig.spawnDuration));

        // Spawn: scale in
        if (m_linkedConfig.enableScaleIn)
            _seq.Join(m_parent.DOScale(1f, m_linkedConfig.spawnDuration).SetEase(m_linkedConfig.scaleInEase));

        // Stay
        _seq.AppendInterval(m_linkedConfig.stayDuration);

        // Despawn: fade out
        if (m_linkedConfig.enableFadeOut)
            _seq.Append(text.DOFade(0f, m_linkedConfig.despawnDuration));

        // Despawn: scale out
        if (m_linkedConfig.enableScaleOut)
            _seq.Join(m_parent.DOScale(0f, m_linkedConfig.despawnDuration));

        _seq.OnComplete(() =>
        {
            if (m_linkedPool != null)
                m_linkedPool.Despawn(this);
        });
    }
}