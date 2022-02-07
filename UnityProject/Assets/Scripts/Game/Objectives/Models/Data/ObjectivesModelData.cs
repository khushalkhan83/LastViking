using Core.Storage;
using Game.Objectives.Data;
using System;

namespace Game.Objectives
{
    [Serializable]
    public class ObjectivesModelData : DataBase, IImmortal
    {
        public ObjectivesData ObjectivesData;

        public void SetObjectivesData(ObjectivesData objectivesData)
        {
            ObjectivesData = objectivesData;
            ChangeData();
        }
    }
}
