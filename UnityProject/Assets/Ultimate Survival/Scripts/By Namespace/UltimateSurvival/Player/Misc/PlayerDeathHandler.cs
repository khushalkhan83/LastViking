using Game.Audio;
using Game.Controllers;
using Game.Models;
using System.Collections;
using UnityEngine;

namespace UltimateSurvival
{
    public class PlayerDeathHandler : PlayerBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private GameObject m_Camera;

        [Header("Audio")]

        [SerializeField] private AudioSource m_AudioSource;

        [SerializeField] private SoundPlayer m_DeathAudio;

        [Header("Stuff To Disable On Death")]

        [SerializeField] private GameObject[] m_ObjectsToDisable;

        [SerializeField] private Behaviour[] m_BehavioursToDisable;

        [SerializeField] private Collider[] m_CollidersToDisable;
        [SerializeField] private CharacterController _characterController;

#pragma warning restore 0649
        #endregion
        public AudioSystem AudioSystem => AudioSystem.Instance;

        private Vector3 m_CamStartPos;
        private Quaternion m_CamStartRot;

        private PlayerHealthModel PlayerHealthModel => ModelsSystem.Instance._playerHealthModel;
        private PlayerFoodModel PlayerFoodModel => ModelsSystem.Instance._playerFoodModel;
        private PlayerWaterModel PlayerWaterModel => ModelsSystem.Instance._playerWaterModel;
        private PlayerStaminaModel PlayerStaminaModel => ModelsSystem.Instance._playerStaminaModel;
        private PlayerDeathModel PlayerDeathModel => ModelsSystem.Instance._playerDeathModel;
        private RandomPlayerAvatarModel RandomPlayerAvatarModel => ModelsSystem.Instance._randomPlayerAvatarModel;
        private PlayerProfileModel PlayerProfileModel => ModelsSystem.Instance._playerProfileModel;
        private StatisticsModel StatisticsModel => ModelsSystem.Instance._statisticsModel;
        private GameTimeModel GameTimeModel => ModelsSystem.Instance._gameTimeModel;
        
        private CharacterController CharacterController => _characterController;

        private void Awake()
        {
            m_CamStartPos = m_Camera.transform.localPosition;
            m_CamStartRot = m_Camera.transform.localRotation;
        }

        public void TutorialDeath()
        {
            AudioSystem.PlayOnce(AudioID.PlayerDeath);
            On_Death();
        }

        public void PreDeath()
        {
            AudioSystem.PlayOnce(AudioID.PlayerDeath);
            PlayerProfileModel.SetPlayerScore((int)CalculatePlayerScore()); //100000 + Random.Range(100, 2000);// 

            On_Death();

            RaycastHit hitInfo;
            Ray ray = new Ray(transform.position + Vector3.up, Vector3.down);
            if (Physics.Raycast(ray, out hitInfo, 1.5f, ~0, QueryTriggerInteraction.Ignore))
            {
                m_Camera.transform.position = hitInfo.point + Vector3.up * 0.1f;
                m_Camera.transform.rotation = Quaternion.Euler(-30f, Random.Range(-180f, 180f), 0f);
            }
        }

        public void Death()
        {
            PlayerDeathModel.UpdateDeathCounter();
            PlayerProfileModel.SetPlayerScore((int) CalculatePlayerScore()); //100000 + Random.Range(100, 2000);// 
            PlayerProfileModel.HasDeadInSession = true;
        }

        private float CalculatePlayerScore()
        {
            float totalDays = GameTimeModel.Days;
            if (totalDays > 365) totalDays = 365;
            uint killed = StatisticsModel.KilledAll;
            float currentDays = GameTimeModel.GetDays(GameTimeModel.Ticks - StatisticsModel.StartAliveTimeTicks);
            if (currentDays > 365) currentDays = 365;

            return (currentDays + killed + 1) * ((totalDays + 1) * 100);
        }

        private void On_Death()
        {
            m_DeathAudio.Play(ItemSelectionMethod.Randomly, m_AudioSource);

            CharacterController.Move(Vector3.zero);

            DisableObjects();
        }

        public void Respawn(float timeImunable)
        {
            StartCoroutine(C_Respawn(timeImunable));
        }

        private IEnumerator C_Respawn(float timeImunable)
        {
            m_Camera.transform.localPosition = m_CamStartPos;
            m_Camera.transform.localRotation = m_CamStartRot;

            EnableObjects();

            yield return new WaitForSeconds(timeImunable);

            Player.Respawn.Send();
        }

        public void DisableObjects()
        {
            foreach (var obj in m_ObjectsToDisable)
                obj.SetActive(false);

            foreach (var behaviour in m_BehavioursToDisable)
                behaviour.enabled = false;

            foreach (var collider in m_CollidersToDisable)
                collider.enabled = false;
        }

        public void EnableObjects()
        {
            foreach (var obj in m_ObjectsToDisable)
                obj.SetActive(true);

            foreach (var behaviour in m_BehavioursToDisable)
                behaviour.enabled = true;
            foreach (var collider in m_CollidersToDisable)
                collider.enabled = true;
        }
    }
}
