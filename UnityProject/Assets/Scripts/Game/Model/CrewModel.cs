using System;
using System.Collections.Generic;
using Game.Providers;
using Game.QuestSystem.Data;
using UnityEngine;

namespace Game.Models
{
    public class CrewModel : MonoBehaviour
    {
        public enum CrewMemberId
        {
            None,
            Fisher,
            TreeMan,
            JumpPirate
        }

        private SideQuestsModel SideQuestsModel => ModelsSystem.Instance._sideQuestsModel;
        private CrewMemberDataProvider CrewMemberDataProvider => ModelsSystem.Instance._crewMemberDataProvider;


        private List<CrewMemberId> _membersInTravel = new List<CrewMemberId>();

        public event Action OnMemberInTravelChanged;

        public bool MemberUnlocked(CrewMemberId id)
        {
            bool unlocked = SideQuestsModel.IsQuestCompleated(CrewMemberDataProvider[id].RelatedQuest);
            return unlocked;
        }

        public bool MemberInTravel(CrewMemberId id)
        {
            bool inTravel = _membersInTravel.Contains(id);
            return inTravel;
        }

        public bool AnyMemberInTravel()
        {
            return _membersInTravel.Count > 0;
        }

        public void CompleateTravel()
        {
            _membersInTravel.Clear();
        }

        public void SetMemberTraveling(CrewMemberId id, bool value)
        {
            if(!value) _membersInTravel.Remove(id);
            else
            {
                if(_membersInTravel.Contains(id)) return;

                _membersInTravel.Add(id);
            }

            OnMemberInTravelChanged?.Invoke();
        }
    }
}
