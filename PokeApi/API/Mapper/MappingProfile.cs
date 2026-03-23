using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PokeApi.API.DTOs;
using PokeApiTeste.Model;

namespace PokeApi.API.Mapper
{
    public sealed class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PokemonColor, ColorPokemonResponseDto>().ReverseMap();

            CreateMap<PokeApiPokemonColorResponse, PokemonColor>().ReverseMap();

            CreateMap<PokemonColor, ColorPokemonResponseDto>()
                .ForMember(dest => dest.PokemonsNames,
                    opt => opt.MapFrom(src => src.PokemonSpecies.Select(p => p.Name)));
        }
    }
}