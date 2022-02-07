namespace Chances
{
    public interface ISmartDice
    {
        float Chance { get; }
        bool GetRollResult();
    }
}