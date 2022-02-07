using Game.AI.BehaviorDesigner;
using Game.Models;
using UnityEngine;
using static Game.Models.CrewModel;
using System.Collections;

namespace Game.QuestSystem.Map.Extra
{
    public class NPCSendOnBase : MonoBehaviour
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private NPCContext _npc;
        [SerializeField] private CrewMemberId _crewMemberId;

        [Space]
        [SerializeField] private Transform _destionation;

        #pragma warning restore 0649
        #endregion

        private CrewModel CrewModel => ModelsSystem.Instance._crewModel;

        private const int coroutineUpdateTime = 2;
        private const int destinationReachedDistance = 10;

        public void Send()
        {
            _npc.gameObject.transform.SetParent(null);

            SetCrewMemberTraveling(true);

            StartCoroutine(CheckDestination());

        }

        private IEnumerator CheckDestination()
        {
            do
            {
                yield return new WaitForSeconds(coroutineUpdateTime);
                Vector3 distance = _destionation.transform.position - _npc.transform.position;
                if(distance.sqrMagnitude < destinationReachedDistance * destinationReachedDistance)
                {
                    SetCrewMemberTraveling(false);
                    yield break;
                }
                
            } while (true);
        }


        private void SetCrewMemberTraveling(bool value)
        {
            _npc.gameObject.SetActive(value);
            CrewModel.SetMemberTraveling(_crewMemberId,value);
        }
    }
}