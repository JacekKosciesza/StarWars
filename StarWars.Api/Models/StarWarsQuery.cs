using AutoMapper;
using GraphQL.Types;
using StarWars.Core.Logic;

namespace StarWars.Api.Models
{
    public class StarWarsQuery : ObjectGraphType
    {
        public StarWarsQuery() { }

        public StarWarsQuery(ITrilogyHeroes trilogyHeroes, Core.Data.IDroidRepository droidRepository, Core.Data.IHumanRepository humanRepository, IMapper mapper)
        {
            Name = "Query";

            Field<CharacterInterface>(
              "hero",
              arguments: new QueryArguments(
                    new QueryArgument<EpisodeEnum> { Name = "episode", Description = "If omitted, returns the hero of the whole saga. If provided, returns the hero of that particular episode." }
              ),
              resolve: context =>
              {
                  var episode = context.GetArgument<Episodes?>("episode");
                  var character = trilogyHeroes.GetHero((int?)episode).Result;
                  var hero = mapper.Map<Character>(character);
                  return hero;
              }
            );

            Field<HumanType>(
                "human",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id", Description = "id of the human" }
                ),
                resolve: context =>
                {
                    var id = context.GetArgument<int>("id");
                    var human = humanRepository.Get(id, include: "HomePlanet").Result;
                    var mapped = mapper.Map<Human>(human);
                    return mapped;
                }
            );
            Field<DroidType>(
                "droid",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id", Description = "id of the droid" }
                ),
                resolve: context =>
                {
                    var id = context.GetArgument<int>("id");
                    var droid = droidRepository.Get(id).Result;
                    var mapped = mapper.Map<Droid>(droid);
                    return mapped;
                }
            );
        }
    }
}
