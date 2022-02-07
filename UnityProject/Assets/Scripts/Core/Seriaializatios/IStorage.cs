namespace Core.Storage
{
    public interface IStorage
    {
        bool IsHasSave<T>(T obj) where T : IUnique;
        void Save<T>(T obj) where T : IUnique;
        void Load<T>(T obj) where T : IUnique;
        void Clear<T>(T obj) where T : IUnique;
        void ClearByUUID(string id);
        void ClearAll();
    }
}
