using Core.Storage;
using Game.Audio;
using Game.Models;
using System;
using System.Linq;
using UltimateSurvival;
using UnityEngine;

public class LoomDeathBoxDroper : MonoBehaviour
{
    #region Data
#pragma warning disable 0649

    [SerializeField] private LoomModel _loomModel;
    [SerializeField] private AudioID _destroySound;

#pragma warning restore 0649
    #endregion

    private IHealth _health;
    private IHealth Health => _health ?? (_health = GetComponentInParent<IHealth>());

    private WorldObjectCreator WorldObjectCreator => ModelsSystem.Instance._worldObjectCreator;

    private AudioSystem AudioSystem => AudioSystem.Instance;

    private LoomModel LoomModel => _loomModel;

    private void OnEnable()
    {
        Health.OnDeath += OnDeathFurnaceHandler;
    }

    private void OnDisable()
    {
        Health.OnDeath -= OnDeathFurnaceHandler;
    }

    private void OnDeathFurnaceHandler()
    {
        foreach (var item in LoomModel.ItemsContainer.Cells.Where(x => x.IsHasItem).Select(x => x.Item))
        {
            DropItem(item);
        }

        LoomModel.ItemsContainer.RemoveAllItems();
        AudioSystem.PlayOnce(_destroySound);
    }

    private void DropItem(SavableItem item)
    {
        var itemPickup = WorldObjectCreator.Create(item.ItemData.WorldObjectID, transform.position, Quaternion.identity).GetComponentInChildren<ItemPickup>();
        itemPickup.SetItemToAdd(item);
    }
}

