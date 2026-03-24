using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Text.Json;
using PokeApi.API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace PokeApi.Tests.Integration;

public class PokemonIntegrationTests : IntegrationTestBase
{
    public PokemonIntegrationTests(WebApplicationFactory<Program> factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetByColor_DeveRetornarSucesso()
    {
        var response = await Client.GetAsync("/api/PokeApi/red/pokemons");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrEmpty(content));
    }

    [Fact]
    public async Task GetByColor_DeveRetornarListaDePokemons()
    {
        var response = await Client.GetAsync("/api/PokeApi/red/pokemons");

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        Assert.False(string.IsNullOrEmpty(content));

        var result = JsonSerializer.Deserialize<ColorPokemonResponseDto>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        Assert.NotNull(result);
        Assert.NotEmpty(result.PokemonsNames);
    }

    [Fact]
    public async Task GetByColor_DevePersistirDadosNoBanco()
    {
        var response = await Client.GetAsync("/api/PokeApi/red/pokemons");

        response.EnsureSuccessStatusCode();

        var context = GetDbContext();

        var color = context.PokemonColors
            .Include(c => c.PokemonSpecies)
            .FirstOrDefault();

        Assert.NotNull(color);
        Assert.Equal("red", color.Name);
        Assert.NotEmpty(color.PokemonSpecies);
    }

    [Fact]
    public async Task GetByColor_NaoDeveDuplicarDados()
    {
        await Client.GetAsync("/api/PokeApi/red/pokemons");
        await Client.GetAsync("/api/PokeApi/red/pokemons");

        var context = GetDbContext();

        Assert.Equal(1, context.PokemonColors.Count());
    }

    [Fact]
    public async Task GetByColor_SegundaChamada_DeveRetornarDadosDoBanco()
    {

        await Client.GetAsync("/api/PokeApi/red/pokemons");

        var response = await Client.GetAsync("/api/PokeApi/red/pokemons");

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        Assert.False(string.IsNullOrEmpty(content));
    }

    [Fact]
    public async Task GetByColor_DeveRetornarNotFound_ParaRotaInvalida()
    {
        var response = await Client.GetAsync("/api/PokeApi/rota-invalida");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
