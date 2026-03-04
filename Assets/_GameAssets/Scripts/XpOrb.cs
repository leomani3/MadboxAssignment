using System.Collections;
using DG.Tweening;
using Lean.Pool;
using UnityEngine;

public class XpOrb : MonoBehaviour, IPoolable
{
    [Header("Spawn Animation")]
    [SerializeField] private float m_scatterRadius = 1.2f;

    [Header("Collect")]
    [SerializeField] private float m_collectDelay = 1.5f;
    [SerializeField] private float m_moveSpeed = 20f;
    
    private LeanXpOrbPool m_pool;

    private Sequence m_spawnSequence;
    private Coroutine m_collectCoroutine;
    private bool m_isCollecting = false;
    
    public void Setup(LeanXpOrbPool pool)
    {
        m_pool = pool;
    }

    public void OnSpawn()
    {
        m_isCollecting = false;
        transform.localScale = Vector3.zero;

        if (LevelManager.Instance != null)
            LevelManager.Instance.onWaveFinished += OnWaveFinished;

        PlaySpawnAnimation();
    }

    public void OnDespawn()
    {
        m_spawnSequence?.Kill();
        if (m_collectCoroutine != null)
        {
            StopCoroutine(m_collectCoroutine);
            m_collectCoroutine = null;
        }

        if (LevelManager.Instance != null)
            LevelManager.Instance.onWaveFinished -= OnWaveFinished;
    }

    private void PlaySpawnAnimation()
    {
        m_spawnSequence?.Kill();

        Vector2 randomCircle = Random.insideUnitCircle.normalized * Random.Range(m_scatterRadius * 0.5f, m_scatterRadius);
        Vector3 scatterTarget = new Vector3(
            transform.position.x + randomCircle.x,
            transform.position.y,
            transform.position.z + randomCircle.y
        );

        m_spawnSequence = DOTween.Sequence();
        m_spawnSequence.Append(transform.DOJump(scatterTarget, 3f, 1, 0.3f));
        m_spawnSequence.Join(transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutBack));
    }

    private void OnWaveFinished()
    {
        if (m_isCollecting) return;

        Entity player = EntityManager.Instance?.Player;
        if (player == null) return;

        MoveToPlayer(player.transform);
    }

    private void MoveToPlayer(Transform playerTransform)
    {
        m_isCollecting = true;
        
        if (m_collectCoroutine != null)
            StopCoroutine(m_collectCoroutine);
        
        if (m_spawnSequence != null && m_spawnSequence.IsActive() && m_spawnSequence.IsPlaying())
        {
            m_spawnSequence.OnComplete(() => 
            {
                m_collectCoroutine = StartCoroutine(CollectRoutine(playerTransform));
            });
        }
        else
        {
            m_spawnSequence?.Kill();
            m_collectCoroutine = StartCoroutine(CollectRoutine(playerTransform));
        }
    }

    private IEnumerator CollectRoutine(Transform playerTransform)
    {
        yield return new WaitForSeconds(m_collectDelay);

        while (playerTransform != null)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                playerTransform.position,
                m_moveSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, playerTransform.position) < 0.05f)
                break;

            yield return null;
        }

        OnOrbCollected();
    }

    private void OnOrbCollected()
    {
        m_pool.Despawn(this);
    }
}