using Lean.Pool;
using UnityEngine;

namespace Pinpin
{
	public class UIFloatingTextPoolRefSetter : MonoBehaviour
	{
		[SerializeField] private UIFloatingTextPoolRef poolRef;

        private void Awake()
        {
            poolRef.pool = GetComponent<LeanUIFloatingTextPool>();
        }
    }
}