namespace Game.Models
{
    public interface IVersionModel
    {
        bool IsLegal { get; }
        string PathVerions { get; }
        string PathStore { get; }
        string VersionLast { get; }

        void SetIsLegal(bool isLegal);
    }
}