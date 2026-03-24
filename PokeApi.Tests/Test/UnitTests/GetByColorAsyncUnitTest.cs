using System.Data.Common;
using AutoMapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using PokeApi.API.DTOs;
using PokeApi.API.Mapper;
using PokeApi.Application.Service;
using PokeApi.Infrastructure.Data;
using PokeApi.Infrastructure.Integrations;
using PokeApiTeste.Application.Exceptions;
using PokeApiTeste.Model;

namespace PokeApi.Tests;

[TestClass]
public class GetByColorAsyncUnitTest
{
    [TestMethod]
    public async Task GetByColorAsync_DeveRetornarPokemonsCadastradosNoBanco_QuandoCorValida()
    {

        using var fixture = new PokemonServiceTestFixture();

        var color = new PokemonColor { Name = "red" };

        color.PokemonSpecies = new List<PokemonSpecies>
        {
            new PokemonSpecies("pikachu") { Color = color },
            new PokemonSpecies("charmander") { Color = color }
        };

        fixture.Context.PokemonColors.Add(color);
        await fixture.Context.SaveChangesAsync();

        var result = await fixture.Service.GetByColorAsync("red");

        Assert.IsNotNull(result);

        fixture.ClientMock.Verify(
            x => x.GetByColorAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [TestMethod]
    public async Task GetByColorAsync_DeveRetornarPokemonsDiretamenteDaApi_QuandoCorValida()
    {

        using var fixture = new PokemonServiceTestFixture();

        fixture.ClientMock
            .Setup(x => x.GetByColorAsync("red", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PokeApiPokemonColorResponse
            {
                Id = 1,
                Name = "red",
                PokemonSpecies = new List<PokeApiColors>
                {
                    new PokeApiColors
                    {
                        Name = "pikachu",
                        Url = "https://pokeapi.co/api/v2/pokemon-species/pikachu"
                    },
                    new PokeApiColors
                    {
                        Name = "charmander",
                        Url = "https://pokeapi.co/api/v2/pokemon-species/charmander"
                    }
                }
            });

        var result = await fixture.Service.GetByColorAsync("red");

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.PokemonsNames.Count);


        fixture.ClientMock.Verify(
            x => x.GetByColorAsync("red", It.IsAny<CancellationToken>()),
            Times.Once
        );


        var savedColor = fixture.Context.PokemonColors.First();

        Assert.AreEqual("red", savedColor.Name);
        Assert.AreEqual(1, fixture.Context.PokemonColors.Count());

        var species = fixture.Context.PokemonSpecies.ToList();

        Assert.AreEqual(2, species.Count);
        Assert.IsTrue(species.Any(s => s.Name == "pikachu"));
        Assert.IsTrue(species.Any(s => s.Name == "charmander"));
    }

    [TestMethod]
    public async Task GetByColorAsync_NaoDeveDuplicarDados_QuandoExistirNoBanco()
    {

        using var fixture = new PokemonServiceTestFixture();

        fixture.ClientMock
            .Setup(x => x.GetByColorAsync("red", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PokeApiPokemonColorResponse
            {
                Id = 1,
                Name = "red",
                PokemonSpecies = new List<PokeApiColors>
                {
                    new PokeApiColors
                    {
                        Name = "pikachu",
                        Url = "https://pokeapi.co/api/v2/pokemon-species/pikachu"
                    },
                    new PokeApiColors
                    {
                        Name = "charmander",
                        Url = "https://pokeapi.co/api/v2/pokemon-species/charmander"
                    }
                }
            });

        await fixture.Service.GetByColorAsync("red");
        await fixture.Service.GetByColorAsync("red");

        Assert.AreEqual(1, fixture.Context.PokemonColors.Count());

        fixture.ClientMock.Verify(
            x => x.GetByColorAsync("red", It.IsAny<CancellationToken>()),
            Times.Once
            );
    }

    [TestMethod]
    public async Task GetByColorAsync_NaoDeveAceitarParametroNull()
    {
        using var fixture = new PokemonServiceTestFixture();

        await Assert.ThrowsExceptionAsync<PokeApiException>(
            () => fixture.Service.GetByColorAsync(null!)
        );

        fixture.ClientMock.Verify(
            x => x.GetByColorAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [TestMethod]
    public async Task GetByColorAsync_DeveLancarExcecao_QuandoStringVazia()
    {
        using var fixture = new PokemonServiceTestFixture();

        await Assert.ThrowsExceptionAsync<PokeApiException>(
            () => fixture.Service.GetByColorAsync("")
        );

        fixture.ClientMock.Verify(
            x => x.GetByColorAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [TestMethod]
    public async Task GetByColorAsync_DeveLancarArgumentNullException_QuandoApiRetornarNull()
    {
        using var fixture = new PokemonServiceTestFixture();

        fixture.ClientMock
               .Setup(x => x.GetByColorAsync("red", It.IsAny<CancellationToken>()))
               .ReturnsAsync((PokeApiPokemonColorResponse?)null);

        Func<Task> Act = () => fixture.Service.GetByColorAsync("red");

        await Assert.ThrowsExceptionAsync<ArgumentNullException>(Act);

        fixture.ClientMock.Verify(
            x => x.GetByColorAsync("red", It.IsAny<CancellationToken>()),
            Times.Once
        );

        Assert.AreEqual(0, fixture.Context.PokemonColors.Count());
    }

    [TestMethod]
    public async Task GetByColorAsync_DeveLancarPokeApiException_QuandoListaForNull()
    {
        using var fixture = new PokemonServiceTestFixture();

        fixture.ClientMock
               .Setup(x => x.GetByColorAsync("red", It.IsAny<CancellationToken>()))
               .ReturnsAsync(new PokeApiPokemonColorResponse
               {
                   Id = 1,
                   Name = "red",
                   PokemonSpecies = null!
               });

        Func<Task> Act = () => fixture.Service.GetByColorAsync("red");

        await Assert.ThrowsExceptionAsync<PokeApiException>(Act);

        fixture.ClientMock.Verify(
            x => x.GetByColorAsync("red", It.IsAny<CancellationToken>()),
            Times.Once
        );

        Assert.AreEqual(0, fixture.Context.PokemonColors.Count());
    }

    [TestMethod]
    public async Task GetByColorAsync_DeveLancarPokeApiException_QuandoListaForVazia()
    {
        using var fixture = new PokemonServiceTestFixture();

        fixture.ClientMock
               .Setup(x => x.GetByColorAsync("red", It.IsAny<CancellationToken>()))
               .ReturnsAsync(new PokeApiPokemonColorResponse
               {
                   Id = 1,
                   Name = "red",
                   PokemonSpecies = new List<PokeApiColors>()
               });

        Func<Task> Act = () => fixture.Service.GetByColorAsync("red");

        await Assert.ThrowsExceptionAsync<PokeApiException>(Act);

        fixture.ClientMock.Verify(
            x => x.GetByColorAsync("red", It.IsAny<CancellationToken>()),
            Times.Once
        );

        Assert.AreEqual(0, fixture.Context.PokemonColors.Count());
    }

    [TestMethod]
    public async Task GetByColorAsync_DeveLancarInvalidColorNameException_QuandoNomeDaCorForNull()
    {
        using var fixture = new PokemonServiceTestFixture();

        fixture.ClientMock
            .Setup(x => x.GetByColorAsync("red", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PokeApiPokemonColorResponse
            {
                Id = 1,
                Name = null!,
                PokemonSpecies = new List<PokeApiColors>
                {
                    new() { Name = "pikachu" }
                }
            });

        Func<Task> Act = () => fixture.Service.GetByColorAsync("red");

        await Assert.ThrowsExceptionAsync<InvalidColorNameException>(Act);

        fixture.ClientMock.Verify(
            x => x.GetByColorAsync("red", It.IsAny<CancellationToken>()),
            Times.Once
        );

        Assert.AreEqual(0, fixture.Context.PokemonColors.Count());
    }

    [TestMethod]
    public async Task GetByColorAsync_DeveLancarInvalidColorNameException_QuandoNomeDaCorForVazioOuEspacos()
    {
        using var fixture = new PokemonServiceTestFixture();

        fixture.ClientMock
               .Setup(x => x.GetByColorAsync("red", It.IsAny<CancellationToken>()))
               .ReturnsAsync(new PokeApiPokemonColorResponse
               {
                   Id = 1,
                   Name = "    ",
                   PokemonSpecies = new List<PokeApiColors>
                   {
                       new() { Name = "Pikachu"}
                   }
               });

        Func<Task> Act = () => fixture.Service.GetByColorAsync("red");

        await Assert.ThrowsExceptionAsync<InvalidColorNameException>(Act);

        fixture.ClientMock.Verify(
            x => x.GetByColorAsync("red", It.IsAny<CancellationToken>()),
            Times.Once
        );

        Assert.AreEqual(0, fixture.Context.PokemonColors.Count());
    }

    [TestMethod]
    public async Task GetByColorAsync_DeveFiltrarENormalizarDadosInvalidos_DaApi()
    {
        using var fixture = new PokemonServiceTestFixture();

        fixture.ClientMock
            .Setup(x => x.GetByColorAsync("red", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PokeApiPokemonColorResponse
            {
                Id = 1,
                Name = "red",
                PokemonSpecies = new List<PokeApiColors>
                {
                new() { Name = "pikachu" },
                new() { Name = "PIKACHU" },
                new() { Name = "  pikachu  " },
                new() { Name = "charmander" },
                new() { Name = "" },
                new() { Name = "   " },
                new() { Name = null! }
                }
            });

        var result = await fixture.Service.GetByColorAsync("red");

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.PokemonsNames.Count);

        var species = fixture.Context.PokemonSpecies.ToList();

        Assert.AreEqual(2, species.Count);

        Assert.IsTrue(species.Any(s => s.Name == "pikachu"));
        Assert.IsTrue(species.Any(s => s.Name == "charmander"));

        Assert.IsTrue(species.All(s => s.Name == s.Name.ToLower()));
        Assert.IsTrue(species.All(s => !string.IsNullOrWhiteSpace(s.Name)));
    }

    [TestMethod]
    public async Task GetByColorAsync_NaoDeveSalvarPokemonsDuplicados_QuandoApiRetornarDuplicados()
    {
        using var fixture = new PokemonServiceTestFixture();

        fixture.ClientMock
            .Setup(x => x.GetByColorAsync("red", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PokeApiPokemonColorResponse
            {
                Id = 1,
                Name = "red",
                PokemonSpecies = new List<PokeApiColors>
                {
                new() { Name = "pikachu" },
                new() { Name = "pikachu" },
                new() { Name = "pikachu" },
                new() { Name = "charmander" },
                new() { Name = "charmander" }
                }
            });

        var result = await fixture.Service.GetByColorAsync("red");

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.PokemonsNames.Count);

        var species = fixture.Context.PokemonSpecies.ToList();

        Assert.AreEqual(2, species.Count);

        Assert.IsTrue(species.Any(s => s.Name == "pikachu"));
        Assert.IsTrue(species.Any(s => s.Name == "charmander"));
    }

    [TestMethod]
    public async Task GetByColorAsync_DeveManterRelacionamentoCorreto_EntreColorEPokemonSpecies()
    {
        using var fixture = new PokemonServiceTestFixture();

        fixture.ClientMock
            .Setup(x => x.GetByColorAsync("red", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PokeApiPokemonColorResponse
            {
                Id = 1,
                Name = "red",
                PokemonSpecies = new List<PokeApiColors>
                {
                new() { Name = "pikachu" },
                new() { Name = "charmander" }
                }
            });

        await fixture.Service.GetByColorAsync("red");

        var color = fixture.Context.PokemonColors
            .Include(c => c.PokemonSpecies)
            .First();

        Assert.IsNotNull(color);
        Assert.AreEqual("red", color.Name);

        Assert.IsNotNull(color.PokemonSpecies);
        Assert.AreEqual(2, color.PokemonSpecies.Count);

        Assert.IsTrue(color.PokemonSpecies.All(p => p.Color != null));
        Assert.IsTrue(color.PokemonSpecies.All(p => p.Color?.Name == "red"));

        foreach (var species in color.PokemonSpecies)
        {
            Assert.AreSame(color, species.Color);
        }
    }
}