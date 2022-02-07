using Game.Models;
using System.Linq;
using UltimateSurvival;
using UnityEngine;

public class ShelterDeathBoxDroper : MonoBehaviour
{
    #region Data
#pragma warning disable 0649

    [SerializeField] private LootObject _lootObject;

#pragma warning restore 0649
    #endregion

    private IHealth _health;
    private IHealth Health => _health ?? (_health = GetComponentInParent<IHealth>());

    private SheltersModel SheltersModel => ModelsSystem.Instance._sheltersModel;

    private WorldObjectCreator WorldObjectCreator => ModelsSystem.Instance._worldObjectCreator;

    private LootObject LootObject => _lootObject;

    private void OnEnable()
    {
        Health.OnDeath += OnDeathShelterHandler;
    }

    private void OnDisable()
    {
        Health.OnDeath -= OnDeathShelterHandler;
    }

    private void OnDeathShelterHandler()
    {
        foreach (var item in LootObject.ItemsContainer.Cells.Where(x => x.IsHasItem).Select(x => x.Item))
        {
            DropItem(item);
        }

        LootObject.ItemsContainer.RemoveAllItems();

        SheltersModel.ShelterModel.Death();
    }

    private void DropItem(SavableItem item)
    {
        var itemPickup = WorldObjectCreator.Create(item.ItemData.WorldObjectID, transform.position, Quaternion.identity).GetComponentInChildren<ItemPickup>();
        itemPickup.SetItemToAdd(item);
    }
}

