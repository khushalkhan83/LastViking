using Game.Models;
using Game.Objectives.Data.Conditions.Static;
using RoboRyanTron.SearchableEnum;
using UnityEngine;

namespace Game.Objectives
{
    [CreateAssetMenu]
    public class ObjectiveData : ScriptableObject
    {
        #region Data
#pragma warning disable 0649
        [SearchableEnum]
        [SerializeField] private LocalizationKeyID _descriptionKeyID;
        [SearchableEnum]
        [SerializeField] private LocalizationKeyID _descriptionCompleteKeyID;
        [SerializeField] private ConditionBaseData[] _conditions;
        [SerializeField] private ActionBaseData[] _beginActions;
        [SerializeField] private ActionBaseData[] _endActions;
        [SerializeField] private ActionBaseData[] _rewards;
        [SerializeField] private bool done;

#pragma warning restore 0649
        #endregion

        public LocalizationKeyID DescriptionKeyID => _descriptionKeyID;
        public LocalizationKeyID DescriptionCompleteKeyID => _descriptionCompleteKeyID;
        public ConditionBaseData[] Conditions => _conditions;
        public ActionBaseData[] BeginActions => _beginActions;
        public ActionBaseData[] EndActions => _endActions;
        public ActionBaseData[] Rewards => _rewards;
        public bool Done {get => done; set => done = value;} //FIXME: refactor
    }
}
