namespace Game.Progressables
{
    public class NonSavableProgressSpawner : ProgressSpawner
    {
        protected override bool CanSpawn() => ProgressStatus == ProgressStatus.NotInProgress || ProgressStatus == ProgressStatus.InProgress;

        protected override void CreateInstance()
        {
            Instance = WorldObjectCreator.CreateAsSpawnable(WorldObjectID, transform.position, transform.rotation, transform.localScale, DataProcessing, transform);
            Instance.OnDelete += OnDeleteInstanceHandler;
            ProgressStatus = ProgressStatus.InProgress;
        }

        private void OnDeleteInstanceHandler()
        {
            Instance.OnDelete -= OnDeleteInstanceHandler;
            ProgressStatus = ProgressStatus.WaitForResetProgress;
        }
    }
}
