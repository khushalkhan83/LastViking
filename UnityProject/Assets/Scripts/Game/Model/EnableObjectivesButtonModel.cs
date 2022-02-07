using UnityEngine;

namespace Game.Models
{
    public class EnableObjectivesButtonModel : MonoBehaviour
    {
        public bool IsFirstShow { get; private set; }

        public void SetFirst() => IsFirstShow = true;
        public void ResetFirst() => IsFirstShow = false;
    }
}
