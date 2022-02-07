using Core.Providers;
using Game.Controllers;
using Game.Controllers.Controllers.States;
using UnityEngine;
using static Game.Models.CrewModel;

namespace Game.Providers
{
    [CreateAssetMenu(fileName = "SO_Providers_crewMembersData_new", menuName = "Providers/CrewMembersData", order = 0)]
    public class CrewMemberDataProvider : ProviderScriptable<CrewMemberId, CrewMemberData>
    {
        [EnumNamedArray(typeof(CrewMemberId))]

        [SerializeField] private CrewMemberData[] _data;

        public override CrewMemberData this[CrewMemberId key] => _data[((int)(object)key - 1)];
    }
}
