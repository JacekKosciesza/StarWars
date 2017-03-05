namespace StarWars.Core.Models
{
    public class CharacterFriend
    {
        public int CharacterId { get; set; }
        public Character Character { get; set; }

        public int FriendId { get; set; }
        public Character Friend { get; set; }
    }
}
