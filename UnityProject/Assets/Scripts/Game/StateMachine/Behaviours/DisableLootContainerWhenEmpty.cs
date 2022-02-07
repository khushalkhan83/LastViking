using UltimateSurvival;
using UnityEngine;

namespace Game.StateMachine.Behaviours
{
    public class DisableLootContainerWhenEmpty : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private GameObject _lootContainerObject;
        [SerializeField] private LootObject _lootObject;

#pragma warning restore 0649
        #endregion

        public GameObject LootContainerObject => _lootContainerObject;
        public LootObject LootObject => _lootObject;

        private void OnEnable()
        {
            LootObject.OnClose += OnCloseLootObjectHandler;
        }

        private void OnDisable()
        {
            LootObject.OnClose -= OnCloseLootObjectHandler;
        }

        private void OnCloseLootObjectHandler()
        {
            if (LootObject.IsEmpty)
            {
                LootContainerObject.SetActive(false);
            }
        }
    }
}
