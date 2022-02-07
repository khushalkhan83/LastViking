namespace Game.Objectives.Conditions.Controllers
{
    public interface IConditionController
    {
        void Register(ConditionModel conditionModel);
        void Unregister(ConditionModel conditionModel);
    }
}
