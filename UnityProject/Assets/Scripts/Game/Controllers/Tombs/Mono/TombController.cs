using Game.Models;
using System.Collections;
using UnityEngine;

namespace Game.Controllers
{
    public class TombController : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private TombModel _tombModel;
        //[SerializeField] private  WorldObjectModel _worldObjectModel; // удален из-за проблем с сериализацией

#pragma warning restore 0649
        #endregion

        private TombModel TombModel => _tombModel;
        private WorldObjectModel WorldObjectModel => GetComponent<WorldObjectModel>(); // hack
        private GameTimeModel GameTimeModel => ModelsSystem.Instance._gameTimeModel;
        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;
        private ItemsDB ItemsDB => ModelsSystem.Instance._itemsDB;

        #region MonoBehaviour
        private void Start()
        {
            LoadTombItems();
        }

        private void OnEnable()
        {
            TombModel.OnDestroyTomb += ReleaseObject;
        }
        private void OnDisable()
        {
            if (TombModel)
            {
                TombModel.OnDestroyTomb -= ReleaseObject;
            }
        }
            
        #endregion

        public void LoadTombItems()
        {
            foreach (var item in TombModel.Items)
            {
                item.ItemData = ItemsDB.GetItem(item.Id);
            }
        }

      
        private void ReleaseObject()
        {
            WorldObjectModel.Delete();

            Destroy(gameObject);
        }
    }
}
