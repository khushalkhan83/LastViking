using Game.Objectives.Controllers;

namespace Game.Objectives.Actions.Controllers
{
    public abstract class BaseActionController<D> : IActionController
        where D : ActionBaseData
    {
        protected abstract void Action(D actionData);

        public void Action(ActionBaseData actionBaseData) => Action((D)actionBaseData);
    }
}
