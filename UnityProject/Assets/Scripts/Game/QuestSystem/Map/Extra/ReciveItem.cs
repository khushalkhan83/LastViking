using System.Collections.Generic;
using Game.Models;
using NaughtyAttributes;
using UltimateSurvival;
using UnityEngine;
using UnityEngine.Events;

namespace Game.QuestSystem.Map.Extra
{
    public class ReciveItem : MonoBehaviour
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private string itemName;
        [SerializeField] private bool autoEquip;
        [SerializeField] private bool modifyProperties;
        
        [ShowIf("modifyProperties")]
        [SerializeField] private List<ItemProperty.Value> modifiedPropertyValues;
        [SerializeField] private UnityEvent onRewardRecived;
        
        #pragma warning restore 0649
        #endregion

        private InventoryOperationsModel InventoryOperationsModel => ModelsSystem.Instance._inventoryOperationsModel;
        public void Recive()
        {
            InventoryOperationsModel.ItemConfig config = null;
            if(modifyProperties && modifiedPropertyValues.Count > 0)
            {
                Dictionary<string,object> modifiedProperties = new Dictionary<string, object>();
                foreach(var property in modifiedPropertyValues)
                {
                    object value = null;
                    if (property.Type == ItemProperty.Type.Int)
                        value = property.Int.Current;
                    if (property.Type ==  ItemProperty.Type.Float)
                        value = property.Float.Current;
                    if (property.Type ==  ItemProperty.Type.FloatRange)
                        value = property.FloatRange.Current;
                    if (property.Type ==  ItemProperty.Type.IntRange)
                        value = property.IntRange.Current;
                    
                    if(value != null)
                    {
                        modifiedProperties.Add(property.Name, value);
                    }
                }
                
                config = new InventoryOperationsModel.ItemConfig(modifiedProperties);
            }
            InventoryOperationsModel.AddItemToPlayer(itemName,1, null, config);
            
            if(autoEquip)
            {
                InventoryOperationsModel.TryEquipItem(itemName);
            }

            onRewardRecived?.Invoke();
        }
    }
}