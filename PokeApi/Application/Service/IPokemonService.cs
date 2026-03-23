using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokeApi.API.DTOs;

namespace PokeApi.Application.Service
{
    public interface IPokemonService
    {
        public Task<ColorPokemonResponseDto> GetByColorAsync(
            string nameColor, CancellationToken cancellationToken = default
        );
    }
}