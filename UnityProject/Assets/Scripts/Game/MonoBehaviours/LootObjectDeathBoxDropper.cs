using Game.Models;
using System.Linq;
using UltimateSurvival;
using UnityEngine;

public class LootObjectDeathBoxDropper : MonoBehaviour
{
    #region Data
#pragma warning disable 0649

    [SerializeField] private LootObject _lootObject;

#pragma warning restore 0649
    #endregion

    private WorldObjectModel _worldObjectModel;
    private WorldObjectModel WorldObjectModel => _worldObjectModel ?? (_worldObjectModel = GetComponentInParent<WorldObjectModel>());

    private WorldObjectCreator WorldObjectCreator => ModelsSystem.Instance._worldObjectCreator;

    private LootObject LootObject => _lootObject;

    private void OnEnable()
    {
        WorldObjectModel.OnPreDelete += OnPreDeleteLootObject;
    }

    private void OnDisable()
    {
        WorldObjectModel.OnPreDelete -= OnPreDeleteLootObject;
    }

    private void OnPreDeleteLootObject()
    {
        _lootObject.LoadData();
        foreach (var item in LootObject.ItemsContainer.Cells.Where(x => x.IsHasItem).Select(x => x.Item))
        {
            DropItem(item);
        }
         
        LootObject.ItemsContainer.RemoveAllItems();
    }

    private void DropItem(SavableItem item)
    {
        Vector3 dropPosition = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
        var itemPickup = WorldObjectCreator.Create(item.ItemData.WorldObjectID, dropPosition, Quaternion.identity).GetComponentInChildren<ItemPickup>();
        itemPickup.SetItemToAdd(item);
    }
}

