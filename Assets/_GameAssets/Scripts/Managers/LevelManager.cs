using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private EntityPoolRef m_playerPoolRef;
    [SerializeField] private Transform m_playerSpawnPoint;
    [SerializeField] private BoxCollider m_cameraBoundingBox;

    [Header("Waves")]
    [SerializeField] private LevelData m_levelData;
    [SerializeField] private BoxCollider m_enemySpawnVolume;

    private Entity m_playerEntity;

    private int m_currentWaveIndex = -1;
    private int m_aliveEnemyCount = 0;
    private bool m_levelComplete = false;

    private void Start()
    {
        SpawnPlayer();
        StartEnemyWaves();
    }

    private void SpawnPlayer()
    {
        m_playerEntity = m_playerPoolRef.pool.Spawn(m_playerSpawnPoint.position, Quaternion.identity, m_playerPoolRef.pool.transform);
        m_playerEntity.Setup(m_playerPoolRef.pool);
        if (m_playerEntity.TryGetModule(out EntityHealthModule playerHealthModule))
        {
            playerHealthModule.OnDeathStart += PlayerDeathStart;
        }
        CameraManager.Instance.Setup(m_playerEntity.transform, m_cameraBoundingBox);
    }

    private void PlayerDeathStart()
    {
        Debug.Log("PLAYER DEAD");
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

        m_aliveEnemyCount = wave.enemies.Length;
        Debug.Log($"Starting wave {m_currentWaveIndex + 1} / {m_levelData.waves.Length} — {m_aliveEnemyCount} enemies");

        for (int i = 0; i < wave.enemies.Length; i++)
        {
            Vector3 spawnPosition = GetSpawnPosition();
            Entity enemy = wave.enemies[i].EntityData.entityPoolRef.pool.Spawn(spawnPosition, Quaternion.identity, transform);
            enemy.Setup(wave.enemies[i].EntityData.entityPoolRef.pool);

            if (enemy.TryGetModule(out EntityHealthModule healthModule))
            {
                healthModule.OnDeathStart += EnemyDeathStart;
            }
            else
            {
                Debug.LogWarning($"Enemy '{enemy.name}' has no EntityHealthModule — it won't be tracked.");
                m_aliveEnemyCount--;
            }
        }

        // Edge case: all enemies were missing health modules
        if (m_aliveEnemyCount <= 0)
            SpawnNextWave();
    }

    private void EnemyDeathStart()
    {
        m_aliveEnemyCount--;
        Debug.Log($"Enemy died. {m_aliveEnemyCount} remaining in wave {m_currentWaveIndex + 1}.");

        if (m_aliveEnemyCount <= 0)
            SpawnNextWave();
    }

    private void OnLevelComplete()
    {
        if (m_levelComplete) return;
        m_levelComplete = true;
        Debug.Log("All waves complete. Level finished!");
    }

    private Vector3 GetSpawnPosition()
    {
        if (m_enemySpawnVolume != null)
        {
            Bounds bounds = m_enemySpawnVolume.bounds;
            float x = Random.Range(bounds.min.x, bounds.max.x);
            float z = Random.Range(bounds.min.z, bounds.max.z);
            return new Vector3(x, bounds.min.y, z);
        }

        Debug.LogWarning("LevelManager: No enemy spawn volume assigned, spawning at origin.");
        return Vector3.zero;
    }
}