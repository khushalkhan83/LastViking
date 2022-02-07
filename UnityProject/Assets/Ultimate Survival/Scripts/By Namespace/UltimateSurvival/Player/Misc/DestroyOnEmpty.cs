using System.Linq;
using UnityEngine;

namespace UltimateSurvival
{
    public class DestroyOnEmpty : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private LootObject _lootObject;
        [SerializeField] private GameObject _prefabRef;

#pragma warning restore 0649
        #endregion

        public LootObject LootObject
        {
            get
            {
                return _lootObject;
            }
        }

        public GameObject PrefabRef
        {
            get
            {
                return _prefabRef;
            }
        }

        private void OnEnable()
        {
            LootObject.OnClose += OnCloseHandler;
        }

        private void OnDisable()
        {
            LootObject.OnClose -= OnCloseHandler;
        }

        private void OnCloseHandler()
        {
            var isCanDestroy = LootObject.ItemsContainer.Cells.Any(x => !x.IsHasItem);
            if (isCanDestroy)
            {
                if (PrefabRef)
                {
                    Instantiate(PrefabRef, transform.position, transform.rotation);
                }

                Destroy(gameObject);
            }

        }
    }
}
