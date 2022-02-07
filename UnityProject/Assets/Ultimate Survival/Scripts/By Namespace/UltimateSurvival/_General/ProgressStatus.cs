namespace Game.Progressables
{
    public enum ProgressStatus
    {
        NotInProgress = 0, // ready for use
        InProgress = 1, // using
        WaitForResetProgress = 2, // after using , waiting for refresh, not ready
    }
}