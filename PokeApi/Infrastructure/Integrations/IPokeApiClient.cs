using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokeApi.API.DTOs;

namespace PokeApi.Infrastructure.Integrations
{
    public interface IPokeApiClient
    {
        Task<PokeApiPokemonColorResponse?> GetByColorAsync(string color, CancellationToken cancellationToken = default); 
    }
}