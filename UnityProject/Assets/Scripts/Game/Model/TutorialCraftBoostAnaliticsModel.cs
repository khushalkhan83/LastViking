using System;
using UnityEngine;

namespace Game.Models
{
    public class TutorialCraftBoostAnaliticsModel : MonoBehaviour
    {
        public event Action<bool> OnItemCrafted;

        public void SetItemCrafted(bool boostUsed) => OnItemCrafted?.Invoke(boostUsed);
    }
}
