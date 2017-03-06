using StarWars.Core.Data;
using System.Collections.Generic;

namespace StarWars.Core.Models
{
    public class Planet : IEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Human> Humans { get; set; }
    }
}
