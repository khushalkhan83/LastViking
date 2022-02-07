using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Game.Models;

namespace UltimateSurvival
{
    public class MineableObjectDiggingPlace : MineableObject, IOutlineTarget
    {
        [SerializeField] private float _initialAmount = 0;
        [SerializeField] private float _durationRespawn = 0;
        [SerializeField] private long _timeSpawnTicks = 0;
        [SerializeField] private CellSpawnSettings[] _dropItmesSettings;
        [SerializeField] private Transform _viewTransform = default;
        [SerializeField] private Collider _hitBoxCollider = default;
        [SerializeField] private List<Renderer> _renderers;

        private Coroutine spawnProcessCoroutine = null;

        public event Action<IOutlineTarget> OnUpdateRendererList;

        public float InitialAmount => _initialAmount;
        public long TimeSpawnTicks => _timeSpawnTicks;

        private GameTimeModel GameTimeModel => ModelsSystem.Instance._gameTimeModel;


        public void LoadDataFromSave(float resourceValue, long timeSpawnTicks)
        {
            Amount = resourceValue;
            _timeSpawnTicks = timeSpawnTicks;

            if(GameTimeModel.RealTimeNowTick >= TimeSpawnTicks)
            {
                Respawn();
            }
            else
            {
                HideView();
                SpawnProcess();
            }
        }

        public override void OnToolHit(Ray cameraRay, RaycastHit hitInfo, ExtractionSetting[] settings)
        {
            // Is the tool used good for destroying this type of object?
            var tool = settings.FirstOrDefault(x => x.ToolID == RequiredToolPurpose);

            // If so, do damage, and also give the player loot.
            if (tool != null)
            {
                var extraction = ResourcesExtractionModel.GetModifier(tool.ToolID) * tool.ExtractionRate;
                
                int oldAmount = Mathf.CeilToInt(Amount);
                DecreaseAllAmount(extraction);
                int newAmount = Mathf.CeilToInt(Amount);

                if(oldAmount > 0 && newAmount <= 0)
                {
                    DropItems();
                }
            }
        }

        private void DropItems()
        {
            foreach (var cellSettings in _dropItmesSettings)
            {
                var item = cellSettings.GenerateItem(ItemsDB.ItemDatabase);
                DropItemModel.DropItemFloating(item, DropLootPoint, true, true);
            }
        }

        protected override void DestroyObject()
        {
            if (m_DestroyedObject)
            {
                Instantiate(m_DestroyedObject, transform.position + transform.TransformVector(OffsetObjectDestroyed), Quaternion.identity);
            }

            HideView();
            StartRespawn();

            if (Destroyed != null)
                Destroyed.Send();
        }

        public int GetColor() => 1;

        public List<Renderer> GetRenderers() => _renderers;

        public bool IsUseWeaponRange() => true;

        private void ShowView()
        {
            _viewTransform.gameObject.SetActive(true);
            _hitBoxCollider.enabled = true;
        }

        private void HideView()
        {
            _viewTransform.gameObject.SetActive(false);
            _hitBoxCollider.enabled = false;
        }

        private void Respawn()
        {
            Amount = _initialAmount;
            ShowView();
        }

        private void SpawnProcess()
        {
            if(spawnProcessCoroutine != null)
            {
                StopCoroutine(spawnProcessCoroutine);
                spawnProcessCoroutine = null;
            }
            spawnProcessCoroutine = StartCoroutine(Spawn(GameTimeModel.GetSecondsTotal(_timeSpawnTicks - GameTimeModel.RealTimeNowTick)));
        }

        private IEnumerator Spawn(float remainingSeconds)
        {
            yield return new WaitForSecondsRealtime(remainingSeconds);
            Respawn();
        }

        private void StartRespawn()
        {
            _timeSpawnTicks = GameTimeModel.TicksRealNow + GameTimeModel.GetTicks(_durationRespawn);
            SpawnProcess();
        }
    }
}
