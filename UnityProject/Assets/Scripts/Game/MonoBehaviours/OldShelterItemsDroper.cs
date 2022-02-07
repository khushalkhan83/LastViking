using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Models;
using UltimateSurvival;
using UnityEngine;

public class OldShelterItemsDroper : MonoBehaviour
{
    [SerializeField] private LootObject lootObject = default;

    private DropContainerModel DropContainerModel => ModelsSystem.Instance._dropContainerModel;
    private PlayerScenesModel PlayerScenesModel => ModelsSystem.Instance._playerScenesModel;

    private void OnEnable()
    {
        PlayerScenesModel.OnEnvironmentLoaded += OnEnvironmentLoaded;
    }

    private void OnDisable()
    {
        PlayerScenesModel.OnEnvironmentLoaded -= OnEnvironmentLoaded;
    }

    private void OnEnvironmentLoaded()
    {
        lootObject.LoadData();
        var items = lootObject.ItemsContainer.Cells.Where(x => x.IsHasItem).Select(x => x.Item).ToList();
        if(items.Count > 0)
        {
            DropContainerModel.DropContainer(transform.position, 0.1f, items);
            lootObject.ItemsContainer.RemoveAllItems();
        }
    }

}
