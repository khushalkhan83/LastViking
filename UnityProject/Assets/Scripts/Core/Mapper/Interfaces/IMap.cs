namespace Core.Mapper
{
    public interface IMap<Key, Value>
    {
        Value this[Key key] { get; }
    }
}
