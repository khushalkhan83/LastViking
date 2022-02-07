using Game.Models;
using UnityEngine;
using UnityEngine.Events;

namespace Game.QuestSystem.Map.Extra
{
    public class Reward : MonoBehaviour
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private string rewardItemName;
        [SerializeField] private int rewardItemCount = 1;
        [SerializeField] private UnityEvent onRewardRecived;
        #pragma warning restore 0649
        #endregion

        private InventoryOperationsModel InventoryOperationsModel => ModelsSystem.Instance._inventoryOperationsModel;
        
        public void ReceiveReward()
        {
            InventoryOperationsModel.AddItemToPlayer(rewardItemName,rewardItemCount);
            onRewardRecived?.Invoke();
        }
    }
}