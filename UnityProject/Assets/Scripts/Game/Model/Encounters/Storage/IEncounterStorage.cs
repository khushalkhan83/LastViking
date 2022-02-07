namespace Encounters
{
    public interface IEncounterStorage
    {
        long LastSpawnTimeTicks { get; set; }
        int CompletionCounter { get; set; }

        void Reset();
    }
}
