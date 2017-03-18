using System.Linq;
using AutoMapper;
using StarWars.Api.Models;

namespace StarWars.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Character
            CreateMap<Core.Models.Character, Character>(MemberList.Destination)
                .ForMember(
                    dest => dest.Friends,
                    opt => opt.MapFrom(src => src.CharacterFriends.Select(x => x.Friend))
                )
                .ForMember(
                    dest => dest.AppearsIn,
                    opt => opt.MapFrom(src => src.CharacterEpisodes.Select(x => x.Episode.Title))
                );

            // Droid
            CreateMap<Core.Models.Droid, Droid>(MemberList.Destination).IncludeBase<Core.Models.Character, Character>();

            // Human
            CreateMap<Core.Models.Human, Human>(MemberList.Destination)
                .IncludeBase<Core.Models.Character, Character>()
                .ForMember(
                    dest => dest.HomePlanet,
                    opt => opt.MapFrom(src => src.HomePlanet.Name)
                );
        }
    }
}
