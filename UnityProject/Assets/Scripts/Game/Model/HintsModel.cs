using System;
using UltimateSurvival;
using UnityEngine;

namespace Game.Models
{
    public class HintsModel : MonoBehaviour
    {
        public event Action<MineableObject> OnCheckHintIsNeeded;
        public event Action<GameObject> OnTryShowMinableToolHint;
        public void TryShowMinableToolHint(GameObject go)
        {
            OnTryShowMinableToolHint.Invoke(go);
        }
        public void CheckHintIsNeeded(MineableObject mineable)
        {
            OnCheckHintIsNeeded.Invoke(mineable);
        }
    }
}
