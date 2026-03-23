using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokeApi.API.DTOs;

namespace PokeApi.Infrastructure.Integrations
{
    public class PokeApiClient : IPokeApiClient
    {
        private readonly HttpClient _client;

        public PokeApiClient(HttpClient client)
        {
            _client = client;
        }

        public Task<PokeApiPokemonColorResponse?> GetByColorAsync(string color, CancellationToken cancellationToken = default)
        {
            return _client.GetFromJsonAsync<PokeApiPokemonColorResponse>(
                $"pokemon-color/{color}/",
                cancellationToken
            );
        }
    }
}