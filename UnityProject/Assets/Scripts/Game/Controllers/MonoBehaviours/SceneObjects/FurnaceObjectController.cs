using Game.Audio;
using Game.Models;
using Game.Views;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers
{
    public class FurnaceObjectController : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private FurnaceObjectView _furnaceObjectView;
        [SerializeField] private FurnaceModel _furnaceModel;
#pragma warning restore 0649
        #endregion

        public FurnaceObjectView FurnaceObjectView => _furnaceObjectView;
        public FurnaceModel FurnaceModel => _furnaceModel;

        private AudioSystem AudioSystem => AudioSystem.Instance;

        private PlayerEventHandler PlayerEventHandler => ModelsSystem.Instance._playerEventHandler;

        private WorldObjectCreator WorldObjectCreator => ModelsSystem.Instance._worldObjectCreator;

        private GameUpdateModel GameUpdateModel => ModelsSystem.Instance._gameUpdateModel;

        public AudioObject AudioBurning { get; private set; }

        private void Start()
        {
            OnChangeFireStateHandler();
        }

        private void OnEnable()
        {
            FurnaceObjectView.IsActiveFire(false);
            FurnaceModel.OnChangeFireState += OnChangeFireStateHandler;
            FurnaceModel.OnDrop += OnDropCampFireHandler;

            if (FurnaceModel.IsFire)
            {
                GameUpdateModel.OnUpdate += FireProcessing;

                if (AudioBurning == null)
                {
                    PlayAudioBurning();
                }
                else
                {
                    ContinuePlayAudioBurning();
                }
            }

            if (FurnaceModel.IsBoost)
            {
                FurnaceModel.OnEndBoost += OnEndBoost;

                GameUpdateModel.OnUpdate += BoostProcessing;
            }
            else
            {
                FurnaceModel.OnStartBoost += OnStartBoost;
            }
        }

        private void OnDisable()
        {
            GameUpdateModel.OnUpdate -= FireProcessing;
            GameUpdateModel.OnUpdate -= BoostProcessing;

            FurnaceModel.OnChangeFireState -= OnChangeFireStateHandler;
            FurnaceModel.OnDrop -= OnDropCampFireHandler;
            FurnaceModel.OnEndBoost -= OnEndBoost;
            FurnaceModel.OnStartBoost -= OnStartBoost;

            if (AudioBurning != null)
            {
                StopAudioBurning();
            }
        }

        private void OnStartBoost()
        {
            FurnaceModel.OnStartBoost -= OnStartBoost;

            FurnaceModel.OnEndBoost += OnEndBoost;
            GameUpdateModel.OnUpdate += BoostProcessing;
        }

        private void OnEndBoost()
        {
            FurnaceModel.OnEndBoost -= OnEndBoost;
            GameUpdateModel.OnUpdate -= BoostProcessing;

            FurnaceModel.OnStartBoost += OnStartBoost;
        }

        private void FireProcessing() => FurnaceModel.FireProcessing(Time.deltaTime);

        private void BoostProcessing() => FurnaceModel.BoostProcessing(Time.deltaTime);

        private void OnChangeFireStateHandler()
        {
            FurnaceObjectView.IsActiveFire(FurnaceModel.IsFire);

            if (FurnaceModel.IsFire)
            {
                GameUpdateModel.OnUpdate += FireProcessing;

                if (AudioBurning == null)
                {
                    PlayAudioBurning();
                }
                else
                {
                    ContinuePlayAudioBurning();
                }
            }
            else
            {
                GameUpdateModel.OnUpdate -= FireProcessing;

                if (AudioBurning != null)
                {
                    StopAudioBurning();
                }
            }
        }

        public void PlayAudioBurning()
        {
            AudioBurning = AudioSystem.CreateAudio(AudioID.Burning);
            AudioBurning.AudioSource.transform.SetParent(FurnaceObjectView.ContainerAudio);
            AudioBurning.AudioSource.transform.localPosition = Vector3.zero;
            AudioBurning.AudioSource.Play();
        }

        private void ContinuePlayAudioBurning()
        {
            AudioBurning.AudioSource.Play();
        }

        private void ReleaseAudio()
        {
            AudioSystem.Release(AudioBurning);
            AudioBurning = null;
        }

        private void StopAudioBurning()
        {
            AudioBurning.AudioSource.Stop();
        }

        private void OnDropCampFireHandler(SavableItem item)
        {
            DropItem(item);
        }

        private void DropItem(SavableItem item)
        {
            var itemPickup = WorldObjectCreator.Create(item.ItemData.WorldObjectID, FurnaceObjectView.DropItemSpawnPoint.position, Quaternion.identity).GetComponentInChildren<ItemPickup>();
            itemPickup.SetItemToAdd(item);
        }
    }
}
