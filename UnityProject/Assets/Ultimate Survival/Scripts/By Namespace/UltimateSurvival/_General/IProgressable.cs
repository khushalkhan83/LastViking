namespace Game.Progressables
{
    public interface IProgressable
    {
        ProgressStatus ProgressStatus { get; set; }
        void ClearProgress();
    }
}