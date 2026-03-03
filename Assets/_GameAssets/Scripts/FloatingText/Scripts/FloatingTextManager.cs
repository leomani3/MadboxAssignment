using MyBox;
using Pinpin;
using UnityEngine;

public class FloatingTextManager : Singleton<FloatingTextManager>
{
    [SerializeField] private UIFloatingTextPoolRef m_uiFloatingTextPoolRef;
    [SerializeField] private FloatingTextConfig m_defaultConfig;

    public void SpawnUIText(Vector3 screenPos, string text, FloatingTextConfig config)
    {
        UIFloatingText spawnedText = m_uiFloatingTextPoolRef.pool.Spawn(screenPos, Quaternion.identity, m_uiFloatingTextPoolRef.pool.transform);
        spawnedText.Init(text, config, m_uiFloatingTextPoolRef.pool);
        spawnedText.Play();
    }
}