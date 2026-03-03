using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

[CreateAssetMenu(menuName = "Config/GameConfig")]
public class GameConfig : ScriptableObject
{
    private static GameConfig _instance;
    public static GameConfig Instance => _instance ?? Load();

    private static GameConfig Load()
    {
        _instance = Resources.Load<GameConfig>("GameConfig");
#if UNITY_EDITOR
        if (_instance == null)
            UnityEngine.Debug.LogError("GameConfig asset not found in Resources folder!");
#endif
        return _instance;
    }


    //-------------------------------------
    

}