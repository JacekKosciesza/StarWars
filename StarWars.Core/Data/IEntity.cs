namespace StarWars.Core.Data
{
    public interface IEntity<TKey>
    {
        TKey Id { get; set; }
    }
}
