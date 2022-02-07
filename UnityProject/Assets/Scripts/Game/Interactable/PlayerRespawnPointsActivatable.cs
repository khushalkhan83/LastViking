using Core;
using Game.Models;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Interactables
{
    public class PlayerRespawnPointsActivatable : Activatable
    {
        #region Data
#pragma warning disable 0649
        [SerializeField] private PlayerRespawnPoints _playerRespawnPoints;

#pragma warning restore 0649
        #endregion

        private Dictionary<Type, object> Links
        {
            get
            {
                var links = new Dictionary<Type, object>()
                {
                    { typeof(PlayerRespawnPoints), _playerRespawnPoints},
                };
                return links;
            }
        }

        private InjectionSystem InjectionSystem => ModelsSystem.Instance._injectionSystem;

        public override void OnActivate() => InjectionSystem.UpdateLinks(Links);
    }
}
