using System;
using UnityEngine;

[Serializable]
public class Wave
{
    public Entity[] enemies;
}

[CreateAssetMenu(fileName = "LevelData", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    public Wave[] waves;
}