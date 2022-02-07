namespace ActionsCollections
{
    public abstract class ActionBase
    {
        public abstract void DoAction();
        public abstract string OperationName { get; }
    }
}