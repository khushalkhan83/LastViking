namespace Chances
{
    public interface IProgressiveSmartDice : ISmartDice
    {
        bool? LastResult { get; }
        int ResultCombo { get; }
    }

}