using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using PokeApi.API.DTOs;
using PokeApi.Tests.Integration;

namespace PokeApi.Tests.Integration;

public class PokemonIntegrationTests : IntegrationTestBase
{
    public PokemonIntegrationTests(WebApplicationFactory<Program> factory)
        : base(factory)
    {
    }

    private async Task<HttpResponseMessage> GetByColorAsync(string color)
    {
        return await Client.GetAsync($"/api/PokeApi/{color}/pokemons");
    }

    private async Task<T?> DeserializeAsync<T>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<T>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );
    }

    private void ResetDatabase()
    {
        var context = GetDbContext();
        context.Database.EnsureDeleted();
        context.Database.Migrate();
    }

    [Fact]
    public async Task GetByColor_ComCorValida_DeveRetornar200EListaValida()
    {
        ResetDatabase();

        var response = await GetByColorAsync("red");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.Equal("application/json; charset=utf-8",
            response.Content.Headers.ContentType?.ToString());

        var result = await DeserializeAsync<ColorPokemonResponseDto>(response);

        Assert.NotNull(result);
        Assert.NotEmpty(result.PokemonsNames);

        Assert.All(result.PokemonsNames, name =>
        {
            Assert.False(string.IsNullOrWhiteSpace(name));
        });
    }

    [Fact]
    public async Task GetByColor_ComCorValida_DevePersistirDadosNoBanco()
    {
        ResetDatabase();

        var response = await GetByColorAsync("red");

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
    public async Task GetByColor_NaoDeveDuplicarDadosNoBanco()
    {
        ResetDatabase();

        await GetByColorAsync("red");
        await GetByColorAsync("red");

        var context = GetDbContext();

        Assert.Equal(1, context.PokemonColors.Count());

        var speciesCount = context.PokemonSpecies.Count();

        Assert.True(speciesCount > 0);
        Assert.Equal(
            speciesCount,
            context.PokemonSpecies.Select(s => s.Name).Distinct().Count()
        );
    }

    [Fact]
    public async Task GetByColor_SegundaChamada_DeveRetornarDadosDoBanco()
    {
        ResetDatabase();

        await GetByColorAsync("red");

        var response = await GetByColorAsync("red");

        response.EnsureSuccessStatusCode();

        var result = await DeserializeAsync<ColorPokemonResponseDto>(response);

        Assert.NotNull(result);
        Assert.NotEmpty(result.PokemonsNames);

        var context = GetDbContext();
        Assert.Equal(1, context.PokemonColors.Count());
    }

    [Fact]
    public async Task GetByColor_RotaInvalida_DeveRetornar404()
    {
        var response = await Client.GetAsync("/api/PokeApi/rota-invalida");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetByColor_CorInvalida_DeveRetornar400ComProblemDetails()
    {
        var response = await GetByColorAsync("invalid");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        Assert.Equal("application/json; charset=utf-8",
            response.Content.Headers.ContentType?.ToString());

        var problem = await DeserializeAsync<ProblemDetails>(response);

        Assert.NotNull(problem);
        Assert.Equal(400, problem.Status);
        Assert.Equal("Invalid External Data", problem.Title);
    }

    [Fact]
    public async Task GetByColor_ComNomeVazio_DeveRetornarErro()
    {
        var response = await GetByColorAsync(" ");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetByColor_ComMaiusculoEespacos_DeveNormalizar()
    {
        var response = await GetByColorAsync("  ReD  ");

        response.EnsureSuccessStatusCode();

        var result = await DeserializeAsync<ColorPokemonResponseDto>(response);

        Assert.NotNull(result);
        Assert.NotEmpty(result.PokemonsNames);
    }

    [Fact]
    public async Task GetByColor_DuasChamadas_DeveRetornarMesmoResultado()
    {
        var first = await GetByColorAsync("red");
        var second = await GetByColorAsync("red");

        var result1 = await DeserializeAsync<ColorPokemonResponseDto>(first);
        var result2 = await DeserializeAsync<ColorPokemonResponseDto>(second);

        Assert.Equal(result1?.PokemonsNames.Count, result2?.PokemonsNames.Count);
    }

    [Fact]
    public async Task GetByColor_DadosPersistidos_DevemSerConsistentes()
    {
        await GetByColorAsync("red");

        var context = GetDbContext();

        var color = context.PokemonColors
            .Include(c => c.PokemonSpecies)
            .First();

        Assert.All(color.PokemonSpecies, p =>
        {
            Assert.False(string.IsNullOrWhiteSpace(p.Name));
        });
    }

    [Fact]
    public async Task GetByColor_Erro_DeveRetornarProblemDetailsValido()
    {
        var response = await GetByColorAsync("invalid");

        var problem = await DeserializeAsync<ProblemDetails>(response);

        Assert.NotNull(problem);
        Assert.NotNull(problem.Title);
        Assert.NotNull(problem.Detail);
        Assert.NotNull(problem.Status);
    }
}