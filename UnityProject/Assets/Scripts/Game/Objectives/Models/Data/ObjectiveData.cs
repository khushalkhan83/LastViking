using System;

namespace Game.Objectives
{

    namespace Data
    {
        [Serializable]
        public struct ObjectiveData
        {
            public ushort Id;
            public ObjectiveID ObjectiveID;
            public ConditionData[] Conditions;
        }
    }
}
