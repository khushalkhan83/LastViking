using Core;
using Core.Controllers;
using Game.Models;
using EasyBuildSystem.Runtimes.Internal.Part;
using EasyBuildSystem.Runtimes.Events;
using EasyBuildSystem.Runtimes.Internal.Socket;
using System;
using UnityEngine;
using System.Linq;

namespace Game.Controllers
{
    public class BuildingProcessController : IBuildingProcessController, IController
    {
        [Inject] public BuildingProcessModel BuildingProcessModel { get; private set; }
        [Inject] public BuildingModeModel BuildingModeModel { get; private set; }
        [Inject] public WorldObjectCreator WorldObjectCreator { get; private set; }

        void IController.Enable() 
        {
            EventHandlers.OnPlacedPart += OnPlacedPart;
            EventHandlers.OnPrePlacePart += OnPrePlacePart;
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            EventHandlers.OnPlacedPart -= OnPlacedPart;
            EventHandlers.OnPrePlacePart -= OnPrePlacePart;
        }

        private void OnPlacedPart(PartBehaviour part, SocketBehaviour socket)
        {
            BuildingProcessModel.PlacePart();
            if(socket != null && SocketHasCustomeScale(socket))
                ResaveScale(part);

            
            if (part.CheckStability())
            {
                BuildingProcessModel.PlacePartStable(part.gameObject);
            }
        }

        private GameObject OnPrePlacePart(PartBehaviour part, Vector3 position, Vector3 rotation)
        {
            var model = part.GetComponent<WorldObjectModel>();
            var result = WorldObjectCreator.Create(model.WorldObjectID, position, Quaternion.Euler(rotation));
            return result.gameObject;
        }

       
        private bool SocketHasCustomeScale(SocketBehaviour socket)
        {
            bool useCustomeScale = false;
            foreach (var partOffset in socket.PartOffsets)
            {
                if(!partOffset.UseCustomScale) continue;

                useCustomeScale = true;
                break;
            }
            return useCustomeScale;
        }

        private void ResaveScale(PartBehaviour part)
        {
            var model = part.GetComponent<WorldObjectModel>();
            model.SetScale(part.transform.localScale);
        }
    }
}
