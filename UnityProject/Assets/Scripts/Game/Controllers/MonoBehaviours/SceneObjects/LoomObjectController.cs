using Game.Audio;
using Game.Models;
using Game.Views;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers
{
    public class LoomObjectController : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private LoomObjectView _loomObjectView;
        [SerializeField] private LoomModel _loomModel;
#pragma warning restore 0649
        #endregion

        public LoomObjectView LoomObjectView => _loomObjectView;
        public LoomModel LoomModel => _loomModel;

        
        private AudioSystem AudioSystem => AudioSystem.Instance;
        private WorldObjectCreator WorldObjectCreator => ModelsSystem.Instance._worldObjectCreator;
        private GameUpdateModel GameUpdateModel => ModelsSystem.Instance._gameUpdateModel;
        public AudioObject AudioWorking { get; private set; }

        private void Start()
        {
            OnChangeWeaveStateHandler();
        }

        private void OnEnable()
        {
            LoomModel.OnChangeWeaveState += OnChangeWeaveStateHandler;
            LoomModel.OnDrop += OnDropWeaveHandler;

            if (LoomModel.IsWeave)
            {
                GameUpdateModel.OnUpdate += WeaweProcessing;

                if (AudioWorking == null)
                {
                    PlayAudioWorking();
                }
                else
                {
                    ContinuePlayAudioWorking();
                }
            }

            if (LoomModel.IsBoost)
            {
                LoomModel.OnEndBoost += OnEndBoost;

                GameUpdateModel.OnUpdate += BoostProcessing;
            }
            else
            {
                LoomModel.OnStartBoost += OnStartBoost;
            }
        }
        
        private void OnDisable()
        {
            GameUpdateModel.OnUpdate -= WeaweProcessing;
            GameUpdateModel.OnUpdate -= BoostProcessing;

            LoomModel.OnChangeWeaveState -= OnChangeWeaveStateHandler;
            LoomModel.OnDrop -= OnDropWeaveHandler;
            LoomModel.OnEndBoost -= OnEndBoost;
            LoomModel.OnStartBoost -= OnStartBoost;

            if (AudioWorking != null)
            {
                StopAudioWorking();
            }
        }

        private void OnStartBoost()
        {
            LoomModel.OnStartBoost -= OnStartBoost;

            LoomModel.OnEndBoost += OnEndBoost;
            GameUpdateModel.OnUpdate += BoostProcessing;
        }

        private void OnEndBoost()
        {
            LoomModel.OnEndBoost -= OnEndBoost;
            GameUpdateModel.OnUpdate -= BoostProcessing;

            LoomModel.OnStartBoost += OnStartBoost;
        }

        private void WeaweProcessing() => LoomModel.WeaweProcessing(Time.deltaTime);

        private void BoostProcessing() => LoomModel.BoostProcessing(Time.deltaTime);

        private void OnChangeWeaveStateHandler()
        {
            LoomObjectView.IsActiveWeave(LoomModel.IsWeave);

            if (LoomModel.IsWeave)
            {
                GameUpdateModel.OnUpdate += WeaweProcessing;

                if (AudioWorking == null)
                {
                    PlayAudioWorking();
                }
                else
                {
                    ContinuePlayAudioWorking();
                }
            }
            else
            {
                GameUpdateModel.OnUpdate -= WeaweProcessing;

                if (AudioWorking != null)
                {
                    StopAudioWorking();
                }
            }
        }

        public void PlayAudioWorking()
        {
            AudioWorking = AudioSystem.CreateAudio(AudioID.Loom);
            AudioWorking.AudioSource.transform.SetParent(LoomObjectView.ContainerAudio);
            AudioWorking.AudioSource.transform.localPosition = Vector3.zero;
            AudioWorking.AudioSource.Play();
        }

        private void ContinuePlayAudioWorking()
        {
            AudioWorking.AudioSource.Play();

        }

        private void ReleaseAudio()
        {
            AudioSystem.Release(AudioWorking);
            AudioWorking = null;
        }

        private void StopAudioWorking()
        {
            AudioWorking.AudioSource.Stop();
        }

        private void OnDropWeaveHandler(SavableItem item)
        {
            DropItem(item);
        }

        private void DropItem(SavableItem item)
        {
            var itemPickup = WorldObjectCreator.Create(item.ItemData.WorldObjectID, LoomObjectView.DropItemSpawnPoint.position, Quaternion.identity).GetComponentInChildren<ItemPickup>();
            itemPickup.SetItemToAdd(item);
        }
    }
}
