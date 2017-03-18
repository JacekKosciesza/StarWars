using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GraphQL.Types;
using StarWars.Core.Data;

namespace StarWars.Api.Models
{
    public class HumanType : ObjectGraphType<Human>
    {
        public HumanType(ICharacterRepository characterRepository, IMapper mapper)
        {
            Name = "Human";

            Field(h => h.Id).Description("The id of the human.");
            Field(h => h.Name, nullable: true).Description("The name of the human.");

            Field<ListGraphType<CharacterInterface>>(
                "friends",
                resolve: context =>
                {
                    var friends = characterRepository.GetFriends(int.Parse(context.Source.Id)).Result;
                    var mapped = mapper.Map<IEnumerable<Character>>(friends);
                    return mapped;
                }
            );

            Field(x => x.AppearsIn).Resolve(
                context =>
                {
                    var episodes = characterRepository.GetEpisodes(int.Parse(context.Source.Id)).Result;
                    var titles = episodes.Select(y => y.Title).ToArray();
                    return titles;
                }
            ).Description("Which movie they appear in.");

            Field(h => h.HomePlanet, nullable: true).Description("The home planet of the human.");

            Interface<CharacterInterface>();
        }
    }
}
