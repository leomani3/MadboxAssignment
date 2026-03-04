using Lean.Pool;
using UnityEngine;

public class XpOrbPoolRefSetter : MonoBehaviour
{
	[SerializeField] private XpOrbPoolRef poolRef;

    private void Awake()
    {
        poolRef.pool = GetComponent<LeanXpOrbPool>();
    }
}