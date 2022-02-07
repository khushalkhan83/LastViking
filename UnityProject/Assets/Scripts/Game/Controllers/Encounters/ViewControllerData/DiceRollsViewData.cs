namespace Game.Encounters.ViewControllerData
{
    public class DiceRollsViewData
    {
        public readonly string header;
        public readonly float chance;
        public readonly bool? lastResult;
        public readonly int combo;

        public DiceRollsViewData(string header, float chance, bool? lastResult, int combo)
        {
            this.header = header;
            this.chance = chance;
            this.lastResult = lastResult;
            this.combo = combo;
        }
    }
}
