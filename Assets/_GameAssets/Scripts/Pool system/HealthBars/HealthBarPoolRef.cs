using Lean.Pool;
using UnityEngine;

[CreateAssetMenu(fileName = "HealthBarPoolRef", menuName = "ScriptableObjects/HealthBarPoolRef")]
public class HealthBarPoolRef : ScriptableObject
{
    public LeanHealthBarPool pool;
}