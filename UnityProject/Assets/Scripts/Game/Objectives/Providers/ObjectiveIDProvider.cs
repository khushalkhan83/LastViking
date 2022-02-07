using System.Collections.Generic;
using System.Linq;

namespace Game.Objectives
{
    public class ObjectiveIDProvider
    {
        private Dictionary<ObjectiveData,ObjectiveID> objectiveIdByData = new Dictionary<ObjectiveData,ObjectiveID>();
        private static ObjectivesProvider _objectivesProvider;

        public ObjectiveIDProvider(ObjectivesProvider objectivesProvider)
        {
            _objectivesProvider = objectivesProvider;
            Reset();
        }

        public ObjectiveID GetID(ObjectiveData data)
        {
            return objectiveIdByData[data];
        }


        public void Reset()
        {
            objectiveIdByData.Clear();

            var allIds = Helpers.EnumsHelper.GetValues<ObjectiveID>().ToList();
            allIds.Remove(ObjectiveID.None);

            foreach (var id in allIds)
            {
                objectiveIdByData.Add(_objectivesProvider[id],id);
            }
        }
    }

}