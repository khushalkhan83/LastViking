using UnityEngine;

namespace Game.Views
{
    public class ShelterUpgradeTableView : MonoBehaviour
    {
        public void SetIsVisible(bool isVisible) => gameObject.SetActive(isVisible);
    }
}
