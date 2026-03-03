using Lean.Pool;
using UnityEngine;

[CreateAssetMenu(fileName = "ParticleSystemPoolRef", menuName = "ScriptableObjects/ParticleSystemPoolRef")]
public class ParticleSystemPoolRef : ScriptableObject
{
    public LeanParticleSystemPool pool;
}