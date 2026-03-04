using System;
using DG.Tweening;
using MyBox;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelManager : Singleton<LevelManager>
{
    public Action onWaveFinished;

    [Header("Player")]
    [SerializeField] private EntityPoolRef m_playerPoolRef;
    [SerializeField] private Transform m_playerSpawnPoint;
    [SerializeField] private BoxCollider m_cameraBoundingBox;

    [Header("Waves")]
    [SerializeField] private LevelData m_levelData;
    [SerializeField] private BoxCollider m_enemySpawnVolume;

    private Entity m_playerEntity;

    private int m_currentWaveIndex = -1;
    private bool m_levelComplete = false;

    private void Awake()
    {
        EntityManager.Instance.onEntityRegistered += OnEntityRegistered;
        EntityManager.Instance.onEntityUnregistered += OnEntityUnregistered;
    }
    
    private void OnDestroy()
    {
        if (EntityManager.Instance != null)
        {
            EntityManager.Instance.onEntityRegistered -= OnEntityRegistered;
            EntityManager.Instance.onEntityUnregistered -= OnEntityUnregistered;
        }
    }

    private void Start()
    {
        SpawnPlayer();
        StartEnemyWaves();
        UIManager.Instance.OpenCanvas(UIManager.Instance.GameCanvas);
    }

    private void OnEntityRegistered(Entity entity)
    {

    }

    private void OnEntityUnregistered(Entity entity)
    {
        if (entity == m_playerEntity)
        {
            PlayerDeathStart();
        }
        else
        {
            if (EntityManager.Instance.Enemies.Count <= 0)
            {
                // Notify all XpOrbs (and anything else listening) that the wave ended
                onWaveFinished?.Invoke();

                SpawnNextWave();
            }
        }
    }

    private void SpawnPlayer()
    {
        m_playerEntity = m_playerPoolRef.pool.Spawn(m_playerSpawnPoint.position, Quaternion.identity, transform);
        m_playerEntity.Setup(m_playerPoolRef.pool);
        CameraManager.Instance.Setup(m_playerEntity.transform, m_cameraBoundingBox);
    }

    private void PlayerDeathStart()
    {
        DOVirtual.DelayedCall(1f, () =>
        {
            UIManager.Instance.OpenCanvas(UIManager.Instance.LoseCanvas);
        });
    }

    private void StartEnemyWaves()
    {
        if (m_levelData == null || m_levelData.waves.Length == 0)
        {
            Debug.LogWarning("LevelManager: No level data or waves assigned.");
            return;
        }

        SpawnNextWave();
    }

    private void SpawnNextWave()
    {
        m_currentWaveIndex++;

        if (m_currentWaveIndex >= m_levelData.waves.Length)
        {
            OnLevelComplete();
            return;
        }

        Wave wave = m_levelData.waves[m_currentWaveIndex];

        if (wave.enemies == null || wave.enemies.Length == 0)
        {
            Debug.LogWarning($"Wave {m_currentWaveIndex} has no enemies. Skipping.");
            SpawnNextWave();
            return;
        }

        for (int i = 0; i < wave.enemies.Length; i++)
        {
            Vector3 spawnPosition = GetSpawnPosition();
            Entity enemy = wave.enemies[i].EntityData.entityPoolRef.pool.Spawn(spawnPosition, Quaternion.identity, transform);
            enemy.Setup(wave.enemies[i].EntityData.entityPoolRef.pool);
        }
    }

    private void OnLevelComplete()
    {
        if (m_levelComplete) return;
        m_levelComplete = true;
        DOVirtual.DelayedCall(2f, () =>
        {
            UIManager.Instance.OpenCanvas(UIManager.Instance.WinCanvas);
        });
    }

    private Vector3 GetSpawnPosition()
    {
        if (m_enemySpawnVolume != null)
        {
            Bounds bounds = m_enemySpawnVolume.bounds;
            float x = Random.Range(bounds.min.x, bounds.max.x);
            float z = Random.Range(bounds.min.z, bounds.max.z);
            return new Vector3(x, 0, z);
        }

        Debug.LogWarning("LevelManager: No enemy spawn volume assigned, spawning at origin.");
        return Vector3.zero;
    }
}