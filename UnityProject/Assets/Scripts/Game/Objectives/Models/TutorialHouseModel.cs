using System;
using CodeStage.AntiCheat.ObscuredTypes;
using Core.Storage;
using SOArchitecture;
using UnityEngine;

namespace Game.Models
{
    public class TutorialHouseModel : MonoBehaviour
    {
        public enum TutorialHousePart 
        {
            None,
            Foundation,
            Walls,
            Roof,
            Door,
        }

         [Serializable]
        public class Data : DataBase
        {
            public ObscuredBool FoundationBuilded;
            public ObscuredBool WallsBuilded;
            public ObscuredBool RoofBuilded;
            public ObscuredBool DoorBuilded;

            public void SetFoundationBuilded(bool value)
            {
                FoundationBuilded = value;
                ChangeData();
            }

            public void SetWallsBuilded(bool value)
            {
                WallsBuilded = value;
                ChangeData();
            }

            public void SetRoofBuilded(bool value)
            {
                RoofBuilded = value;
                ChangeData();
            }

            public void SetDoorBuilded(bool value)
            {
                DoorBuilded = value;
                ChangeData();
            }
        }

        #region Data
    #pragma warning disable 0649

        [SerializeField] Data _data;
        [SerializeField] private int _sellectedCellFoundation = 0;

    #pragma warning restore 0649
        #endregion

        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;

        public Data _Data => _data;
        public int SellectedCellFoundation => _sellectedCellFoundation;

        #region MonoBehaviour
        private void OnEnable() 
        {
            StorageModel.TryProcessing(_data);
        }
            
        #endregion
        public Action<TutorialHousePart, bool> NeedBuildPartChanged;
        public Action<TutorialHousePart> OnPartBuilded;
        public event Action OnPlayerEnterConstructionZone;


        private bool _needBuildFoundation;
        private bool _needBuildWalls;
        private bool _needBuildRoof;
        private bool _needBuildDoor;
        
        public bool FoundationBuilded
        {
            get{ return _data.FoundationBuilded; }
            private set{ _data.SetFoundationBuilded(value); }
        }

        public bool WallsBuilded
        {
            get{ return _data.WallsBuilded; }
            private set{ _data.SetWallsBuilded(value); }
        }

        public bool RoofBuilded
        {
            get{ return _data.RoofBuilded; }
            private set{ _data.SetRoofBuilded(value); }
        }

        public bool DoorBuilded
        {
            get{ return _data.DoorBuilded; }

            private set{ _data.SetDoorBuilded(value); }
        }

        public bool GetNeedBuildPart(TutorialHousePart part)
        {
            switch(part)
            {
                case TutorialHousePart.Foundation:
                    return _needBuildFoundation;
                case TutorialHousePart.Walls:
                    return _needBuildWalls;
                case TutorialHousePart.Roof:
                    return _needBuildRoof;
                case TutorialHousePart.Door:
                    return _needBuildDoor;
                default:
                    return false;
            }
        }

        public void SetNeedBuildPart(TutorialHousePart part, bool value)
        {
            switch(part)
            {
                case TutorialHousePart.Foundation:
                    _needBuildFoundation = value;
                    break;
                case TutorialHousePart.Walls:
                    _needBuildWalls = value;
                    break;
                case TutorialHousePart.Roof:
                    _needBuildRoof = value;
                    break;
                case TutorialHousePart.Door:
                    _needBuildDoor = value;
                    break;
            }
            NeedBuildPartChanged?.Invoke(part, value);
        }

        public bool IsBuilded(TutorialHousePart part)
        {
             switch(part)
            {
                case TutorialHousePart.Foundation:
                    return FoundationBuilded;
                case TutorialHousePart.Walls:
                    return WallsBuilded;
                case TutorialHousePart.Roof:
                    return RoofBuilded;
                case TutorialHousePart.Door:
                    return DoorBuilded;
                default:
                    return false;
            }
        }
        public void BuildPart(TutorialHousePart part)
        {
             switch(part)
            {
                case TutorialHousePart.Foundation:
                    FoundationBuilded = true;
                    break;
                case TutorialHousePart.Walls:
                    WallsBuilded = true;
                    break;
                case TutorialHousePart.Roof:
                    RoofBuilded = true;
                    break;
                case TutorialHousePart.Door:
                    DoorBuilded = true;
                    break;
            }
            OnPartBuilded?.Invoke(part);
        }

        public void PlayerEnterConstructionZone() => OnPlayerEnterConstructionZone?.Invoke();

    }
}
