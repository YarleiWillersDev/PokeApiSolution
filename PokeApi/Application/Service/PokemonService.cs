using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PokeApi.API.DTOs;
using PokeApi.Infrastructure.Data;
using PokeApi.Infrastructure.Integrations;
using PokeApiTeste.Application.Exceptions;
using PokeApiTeste.Model;

namespace PokeApi.Application.Service
{
    public class PokemonService : IPokemonService
    {

        private readonly AppDbContext _context;
        private readonly IPokeApiClient _pokeApi;
        private readonly IMapper _mapper;

        public PokemonService(AppDbContext context, IPokeApiClient pokeApiClient, IMapper mapper)
        {
            _context = context;
            _pokeApi = pokeApiClient;
            _mapper = mapper;
        }

        public async Task<ColorPokemonResponseDto> GetByColorAsync(
            string nameColor, CancellationToken cancellationToken = default)
        {
            var color = NormalizeColorName(nameColor);

            var existing = await FindColorInDatabaseAsync(color, cancellationToken);
            if (existing is not null)
                return _mapper.Map<ColorPokemonResponseDto>(existing);

            var apiResponse = await FetchFromPokeApiAsync(color, cancellationToken);

            PokemonColor colorEntity = await PersistFromApiAsync(apiResponse, cancellationToken);

            return _mapper.Map<ColorPokemonResponseDto>(colorEntity);
        }

        private static string NormalizeColorName(string colorName)
        {
            if (colorName is null)
                throw new PokeApiException(nameof(colorName));

            var color = colorName.Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(color))
                throw new PokeApiException("A cor não pode ser vazia");

            return color;
        }

        private async Task<PokemonColor?> FindColorInDatabaseAsync(string colorName, CancellationToken ct)
        {
            return await _context.PokemonColors
                                 .Include(pc => pc.PokemonSpecies)
                                 .FirstOrDefaultAsync(pc => pc.Name == colorName, ct);

        }

        private async Task<PokeApiPokemonColorResponse> FetchFromPokeApiAsync(string colorName, CancellationToken ct)
        {
            try
            {
                var response = await _pokeApi.GetByColorAsync(colorName, ct);

                if (response is null)
                    throw new InvalidColorNameException();

                if (response.PokemonSpecies == null || !response.PokemonSpecies.Any())
                    throw new InvalidColorNameException("PokéAPI retornou sem pokémons para essa cor");

                return response;
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound || ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    throw new InvalidColorNameException("Cor inválida ou não encontrada na PokeAPI", ex);
                }
                throw new PokeApiException("Erro ao acessar a PokeAPI", ex);
            }
            catch (TaskCanceledException ex)
            {
                throw new PokeApiException("Timeout ao acessar a PokeAPI", ex);
            }
            catch (Exception ex)
            {
                throw new PokeApiException("Erro inesperado ao acessar a PokeAPI", ex);
            }
        }

        private async Task<PokemonColor> PersistFromApiAsync(PokeApiPokemonColorResponse apiResponse, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(apiResponse.Name))
                throw new InvalidColorNameException();

            var color = new PokemonColor
            {
                Name = apiResponse.Name
            };

            var speciesValidas = apiResponse.PokemonSpecies
                .Where(s => !string.IsNullOrWhiteSpace(s.Name))
                .Select(s => s.Name.Trim().ToLowerInvariant())
                .Distinct();

            color.PokemonSpecies = speciesValidas
                .Select(name => new PokemonSpecies(name)
                {
                    Color = color
                })
                .ToList();

            _context.PokemonColors.Add(color);
            await _context.SaveChangesAsync(ct);

            return color;
        }
    }
}