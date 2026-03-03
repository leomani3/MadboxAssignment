using Lean.Pool;
using UnityEngine;

namespace Pinpin
{
    [CreateAssetMenu(fileName = "UIFloatingTextPoolRef", menuName = "ScriptableObjects/UIFloatingTextPoolRef")]
    public class UIFloatingTextPoolRef : ScriptableObject
	{
        public LeanUIFloatingTextPool pool;
	}
}