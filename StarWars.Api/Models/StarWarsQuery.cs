using GraphQL.Types;
using StarWars.Core.Data;

namespace StarWars.Api.Models
{
    public class StarWarsQuery : ObjectGraphType
    {
        private IDroidRepository _droidRepository { get; set; }

        public StarWarsQuery() { }

        public StarWarsQuery(IDroidRepository _droidRepository)
        {
            Field<DroidType>(
              "hero",
              resolve: context => _droidRepository.Get(2001)
            );
        }
    }
}
