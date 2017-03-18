using StarWars.Core.Data;
using System.Collections.Generic;

namespace StarWars.Core.Models
{
    public class Character : IEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<CharacterEpisode> CharacterEpisodes { get; set; }
        public ICollection<CharacterFriend> CharacterFriends { get; set; }
        public ICollection<CharacterFriend> FriendCharacters { get; set; }
    }
}
