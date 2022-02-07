using Game.Audio;
using Game.Models;
using Game.Views;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers
{
    public class CampFireObjectController : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private CampFireObjectView _campFireView;
        [SerializeField] private CampFireModel _campFireModel;

#pragma warning restore 0649
        #endregion

        public CampFireObjectView CampFireObjectView => _campFireView;
        public CampFireModel CampFireModel => _campFireModel;

        private AudioSystem AudioSystem => AudioSystem.Instance;

        private WorldObjectCreator WorldObjectCreator => ModelsSystem.Instance._worldObjectCreator;

        private GameUpdateModel GameUpdateModel => ModelsSystem.Instance._gameUpdateModel;

        private CampFiresModel CampFiresModel => ModelsSystem.Instance._campFiresModel;

        public AudioObject AudioCampBurning { get; private set; }

        private void Start()
        {
            OnChangeFireStateHandler();
        }

        private void OnEnable()
        {
            CampFireObjectView.IsActiveFire(false);
            CampFireModel.OnChangeFireState += OnChangeFireStateHandler;
            CampFireModel.OnDrop += OnDropCampFireHandler;
            CampFireModel.OnCook += OnCookCampFireHandler;

            if (CampFireModel.IsFire)
            {
                GameUpdateModel.OnUpdate += OnUpdate;

                if (AudioCampBurning == null)
                {
                    PlayAudioBurning();
                }
                else
                {
                    ContinuePlayAudioBurning();
                }
            }
        }

        private void OnDisable()
        {
            GameUpdateModel.OnUpdate -= OnUpdate;

            CampFireModel.OnChangeFireState -= OnChangeFireStateHandler;
            CampFireModel.OnDrop -= OnDropCampFireHandler;
            CampFireModel.OnCook -= OnCookCampFireHandler;

            if (AudioCampBurning != null)
            {
                StopAudioBurning();
            }
        }

        private void OnCookCampFireHandler(string itemName, int count) => CampFiresModel.CookItem(itemName, count);

        private void OnChangeFireStateHandler()
        {
            CampFireObjectView.IsActiveFire(CampFireModel.IsFire);

            if (CampFireModel.IsFire)
            {
                GameUpdateModel.OnUpdate += OnUpdate;

                if (AudioCampBurning == null)
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
                GameUpdateModel.OnUpdate -= OnUpdate;

                if (AudioCampBurning != null)
                {
                    StopAudioBurning();
                }
            }
        }

        private void PlayAudioBurning()
        {
            AudioCampBurning = AudioSystem.CreateAudio(AudioID.Burning);
            AudioCampBurning.AudioSource.transform.SetParent(CampFireObjectView.ContainerAudio);
            AudioCampBurning.AudioSource.transform.localPosition = Vector3.zero;
            AudioCampBurning.AudioSource.Play();
        }

        private void ContinuePlayAudioBurning()
        {
            AudioCampBurning.AudioSource.Play();
        }

        private void ReleaseAudio()
        {
            AudioSystem.Release(AudioCampBurning);
            AudioCampBurning = null;
        }

        private void StopAudioBurning()
        {
            AudioCampBurning.AudioSource.Stop();
        }

        private void OnUpdate()
        {
            CampFireModel.OnUpdate(Time.deltaTime);
        }

        private void OnDropCampFireHandler(SavableItem item)
        {
            DropItem(item);
        }

        private void DropItem(SavableItem item)
        {
            var itemPickup = WorldObjectCreator.Create(item.ItemData.WorldObjectID, CampFireObjectView.DropItemSpawnPoint.position, Quaternion.identity).GetComponentInChildren<ItemPickup>();
            itemPickup.SetItemToAdd(item);
        }
    }
}
