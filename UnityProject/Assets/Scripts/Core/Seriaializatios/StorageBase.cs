namespace Core.Storage
{
    public abstract class StorageBase : IStorage
    {
        public abstract void Save<T>(T obj) where T : IUnique;
        public abstract void Load<T>(T obj) where T : IUnique;
        public abstract bool IsHasSave<T>(T obj) where T : IUnique;
        public abstract void Clear<T>(T obj) where T : IUnique;
        public abstract void ClearByUUID(string id);
        public abstract void ClearAll();
    }
}
