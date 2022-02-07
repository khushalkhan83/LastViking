using UnityEngine;
using Game.Models;
using static Game.Models.CrewModel;
using System.Collections;
using Extensions;
using System;

namespace Game.Controllers
{
    public class CrewController : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649
        [SerializeField] private GameObject treeMan;
        [SerializeField] private GameObject fisher;
        [SerializeField] private GameObject jumper;
        [SerializeField] private ColliderTriggerModel colliderTriggerModel;
        [SerializeField] private LayerMask _activatorLayer;
        
#pragma warning restore 0649
        #endregion

        private SideQuestsModel SideQuestsModel => ModelsSystem.Instance._sideQuestsModel;
        private PlayerScenesModel PlayerScenesModel => ModelsSystem.Instance._playerScenesModel;
        private ShelterAttackModeModel ShelterAttackModeModel => ModelsSystem.Instance._shelterAttackModeModel;
        private CrewModel CrewModel => ModelsSystem.Instance._crewModel;

        private bool playerCanSeeCrewMembers {get; set;}
        private event Action OnPlayerCanSeeCrewMembersChanged;


        #region MonoBehaviour

        private void Awake()
        {
            treeMan.SetActive(false);
            fisher.SetActive(false);
            jumper.SetActive(false);
            colliderTriggerModel.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            PlayerScenesModel.OnEnvironmentLoaded += RefreshAll;
            SideQuestsModel.OnDataChanged += RefreshAll;
            ShelterAttackModeModel.OnAttackModeActiveChanged += RefreshAll;
            CrewModel.OnMemberInTravelChanged += RefreshAll;
            OnPlayerCanSeeCrewMembersChanged += RefreshAll;

            colliderTriggerModel.OnEnteredTrigger += OnEnteredTrigger;
            colliderTriggerModel.OnExitedTrigger += OnExitedTrigger;
            
            colliderTriggerModel.gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            PlayerScenesModel.OnEnvironmentLoaded -= RefreshAll;
            SideQuestsModel.OnDataChanged -= RefreshAll;
            ShelterAttackModeModel.OnAttackModeActiveChanged -= RefreshAll;
            CrewModel.OnMemberInTravelChanged -= RefreshAll;
            OnPlayerCanSeeCrewMembersChanged -= RefreshAll;

            colliderTriggerModel.OnEnteredTrigger -= OnEnteredTrigger;
            colliderTriggerModel.OnExitedTrigger -= OnExitedTrigger;

            // will work because this script placed in environment scene. Be carefull if move logic to non monobehaviour controller
            CrewModel.CompleateTravel();

            colliderTriggerModel.gameObject.SetActive(false);
        }

        #endregion

        private void RefreshAll()
        {
            bool underAttack = ShelterAttackModeModel.AttackModeActive;

            RefreshMember(CrewMemberId.Fisher,fisher);
            RefreshMember(CrewMemberId.JumpPirate,jumper);
            RefreshMember(CrewMemberId.TreeMan,treeMan);


            void RefreshMember(CrewMemberId id, GameObject view)
            {
                if(underAttack || !playerCanSeeCrewMembers)
                {
                    view.SetActive(false);
                    return;
                }

                view.SetActive(CrewModel.MemberUnlocked(id) && !CrewModel.MemberInTravel(id));
            }
        }

        private void OnEnteredTrigger(Collider other)
        {
            if (!other.gameObject.InsideLayerMask(_activatorLayer)) return;

            SetPlayerCanSeeCrewMembers(true);
        }

        private void OnExitedTrigger(Collider other)
        {
            if (!other.gameObject.InsideLayerMask(_activatorLayer)) return;

            SetPlayerCanSeeCrewMembers(false);
        }

        public void SetPlayerCanSeeCrewMembers(bool value)
        {
            playerCanSeeCrewMembers = value;
            OnPlayerCanSeeCrewMembersChanged?.Invoke();
        }
    }
}