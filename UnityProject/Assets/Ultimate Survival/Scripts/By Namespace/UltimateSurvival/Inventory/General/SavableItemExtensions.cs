namespace UltimateSurvival
{
    public static class SavableItemExtensions
    {
        public static bool IsCanConsume(this SavableItem item)
        {
            return item.HasProperty("Health Change")
                || item.HasProperty("Hunger Change")
                || item.HasProperty("Thirst Change");
        }

        public static bool IsBroken(this SavableItem item)
        {
            return item.TryGetProperty("Durability", out var durability) && durability.Float.Current <= 0;
        }

        public static bool IsCanStacked(this SavableItem to, SavableItem from)
        {
            return
                to != null
                && from != null
                && to.Id == from.Id
                && to.Count > 0
                && from.Count > 0
                && to.ItemData.StackSize > to.Count;
        }
    }
}
