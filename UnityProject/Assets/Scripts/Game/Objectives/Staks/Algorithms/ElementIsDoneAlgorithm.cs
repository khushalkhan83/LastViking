using System.Linq;
using Extensions;

namespace Game.Objectives.Stacks
{
    public partial class ObjectivesStack
    {
        private bool GetIsDonePerObjectiveDefault(ObjectiveData objectiveData)
        {
            int conditionID = objectiveData.Conditions.FirstOrDefault().Id;
            bool state = ObjectivesProgressModel.GetObjestiveState(conditionID);
            return state;
        }

        private bool GetIsDonePerObjectiveConditional(bool conditionOk, ObjectiveData objectiveData)
        {
            bool state = false;
            if (IsSelected)
                state = GetIsDonePerObjectiveDefault(objectiveData);
            else
                state = false;
            return state;
        }
    }
}