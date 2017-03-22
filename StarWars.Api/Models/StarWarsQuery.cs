using AutoMapper;
using GraphQL.Types;

namespace StarWars.Api.Models
{
    public class StarWarsQuery : ObjectGraphType
    {
        public StarWarsQuery() { }

        public StarWarsQuery(Core.Data.IDroidRepository droidRepository, Core.Data.IHumanRepository humanRepository, IMapper mapper)
        {
            Name = "Query";

            Field<DroidType>(
              "hero",
              arguments: new QueryArguments(
                    new QueryArgument<EpisodeEnum> { Name = "episode", Description = "If omitted, returns the hero of the whole saga. If provided, returns the hero of that particular episode." }
              ),
              resolve: context =>
              {
                  var episode = context.GetArgument<Episodes?>("episode");
                  // TODO: get hero (character) based on episode
                  var r2d2 = droidRepository.Get(2001/*, includes: new [] { "CharacterEpisodes.Episode", "CharacterFriends.Friend" }*/).Result;
                  var droid = mapper.Map<Droid>(r2d2);
                  return droid;
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
                    var human = humanRepository.Get(id, includes: new[] { "CharacterEpisodes.Episode", "CharacterFriends.Friend", "HomePlanet" }).Result;
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
                    var droid = droidRepository.Get(id, includes: new[] { "CharacterEpisodes.Episode", "CharacterFriends.Friend" }).Result;
                    var mapped = mapper.Map<Droid>(droid);
                    return mapped;
                }
            );
        }
    }
}
