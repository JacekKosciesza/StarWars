using System.Collections.Generic;

namespace StarWars.Core.Models
{
    public class Episode
    {
        public int  Id  { get; set; }
        public string Title { get; set; }
        public virtual ICollection<CharacterEpisode> CharacterEpisodes { get; set; }
    }
}
