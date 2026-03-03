using Lean.Pool;
using UnityEngine;

public class ParticleSystemPoolRefSetter : MonoBehaviour
{
	[SerializeField] private ParticleSystemPoolRef poolRef;

    private void Awake()
    {
        poolRef.pool = GetComponent<LeanParticleSystemPool>();
    }
}