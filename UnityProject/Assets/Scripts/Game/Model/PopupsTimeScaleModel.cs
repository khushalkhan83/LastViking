using System;
using System.Collections.Generic;
using Game.Views;
using RoboRyanTron.SearchableEnum;
using UnityEngine;

namespace Game.Models
{
    public class PopupsTimeScaleModel : MonoBehaviour
    {
        [SearchableEnum]
        [SerializeField] private List<ViewConfigID> popupViewIds = default;

        public List<ViewConfigID> PopupViewIds => popupViewIds;
    }
}
