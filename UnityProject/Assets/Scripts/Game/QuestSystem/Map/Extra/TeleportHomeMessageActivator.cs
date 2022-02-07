using System.Collections;
using System.Collections.Generic;
using Game.Models;
using UnityEngine;

namespace Game.QuestSystem.Map.Extra
{
    public class TeleportHomeMessageActivator : MonoBehaviour
    {
        private TeleporHomeModel TeleporHomeModel => ModelsSystem.Instance._teleporHomeModel;

        public void Activate()
        {
            TeleporHomeModel.ShowPopup();
        }
    }
}
