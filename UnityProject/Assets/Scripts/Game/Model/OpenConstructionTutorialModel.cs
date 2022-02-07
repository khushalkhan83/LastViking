using System;
using UnityEngine;

namespace Game.Models
{
    public class OpenConstructionTutorialModel : MonoBehaviour
    {
        public event Action OnPlayerEnterConstructionZone;

        public void PlayerEnterConstructionZone() => OnPlayerEnterConstructionZone?.Invoke();
    }
}
