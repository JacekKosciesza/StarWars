namespace StarWars.Api.Models
{
    public class Character
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Character[] Friends { get; set; }
        public string[] AppearsIn { get; set; }
    }
}
