using System;
using Core;
using Core.Controllers;
using Game.Models;
using UnityEngine;

namespace Game.Controllers
{
    public class EventSystemController : IEventSystemController, IController
    {
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public EventSystemModel EventSystemModel { get; private set; }

        private GameObject _lastSelectedGameObject;

        
        void IController.Enable() 
        {
            GameUpdateModel.OnUpdate += OnUpdate;
        }

        void IController.Start() 
        {
            
        }

        void IController.Disable() 
        {
            GameUpdateModel.OnUpdate -= OnUpdate;
        }

        private void OnUpdate()
        {
            var currentSelectedGameObject = EventSystemModel.EventSystem.currentSelectedGameObject;
            if (currentSelectedGameObject != _lastSelectedGameObject)
            {
                EventSystemModel.SelectionChanged(currentSelectedGameObject,_lastSelectedGameObject);
                _lastSelectedGameObject = currentSelectedGameObject;
            }
        }
    }
}
