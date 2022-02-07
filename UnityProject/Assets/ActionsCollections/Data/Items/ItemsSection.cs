using System.Collections.Generic;
using ActionsCollections;
using CustomeEditorTools;
using Game.Models;
using UnityEngine;

namespace DebugActions
{
    public partial class DebugSections {public static ItemsSection Items {get;} = new ItemsSection();}
    public class ItemsSection : SectionBase
    {
        public override string SectionName => "Items";
        private ItemsGroup ItemsGroup => ModelsSystem.Instance._debugitemsGroup;
        private EditorGameSettings EditorGameSettings => EditorGameSettings.Instance;
        public override List<ActionBase> Actions
        {
            get => GetActions();
        }

        private List<ActionBase> GetActions()
        {
            var answer = new List<ActionBase>();

            answer.Add(new ItemsActionSpawnPrefab("spawn devChest",EditorGameSettings.settings.DebugAssets.DevChest, 2));

            foreach (var itemsList in ItemsGroup.ItemsLists)
            {
                var itemListName = itemsList.name.Replace("SO_items_","");
                answer.Add(new ItemsActionAddItems(itemListName,itemsList.Items));
            }

            return answer;
        }

    }


}