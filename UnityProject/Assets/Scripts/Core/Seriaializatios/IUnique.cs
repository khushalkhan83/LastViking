namespace Core.Storage
{

    public interface IUnique
    {
        string UUID { get; set; }
        uint UUIDPrefix { get; set; }
    }
}
