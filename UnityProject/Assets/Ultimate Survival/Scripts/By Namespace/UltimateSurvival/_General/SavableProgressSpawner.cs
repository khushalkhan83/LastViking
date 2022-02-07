namespace Game.Progressables
{
    public class SavableProgressSpawner : ProgressSpawner
    {
        protected override bool CanSpawn() => ProgressStatus == ProgressStatus.NotInProgress;

        protected override void CreateInstance()
        {
            Instance = WorldObjectCreator.Create(WorldObjectID, transform.position, transform.rotation);
            ProgressStatus = ProgressStatus.WaitForResetProgress;
        }
    }
}
