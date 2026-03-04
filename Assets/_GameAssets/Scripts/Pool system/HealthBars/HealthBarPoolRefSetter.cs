using Lean.Pool;
using UnityEngine;

public class HealthBarPoolRefSetter : MonoBehaviour
{
	[SerializeField] private HealthBarPoolRef poolRef;

    private void Awake()
    {
        poolRef.pool = GetComponent<LeanHealthBarPool>();
    }
}