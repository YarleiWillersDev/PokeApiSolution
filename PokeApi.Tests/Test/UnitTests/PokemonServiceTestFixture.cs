using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using PokeApi.API.Mapper;
using PokeApi.Application.Service;
using PokeApi.Infrastructure.Data;
using PokeApi.Infrastructure.Integrations;

namespace PokeApi.Tests
{
    public class PokemonServiceTestFixture : IDisposable
    {
        public AppDbContext Context { get; private set; }
        public Mock<IPokeApiClient> ClientMock { get; private set; }
        public IMapper Mapper { get; private set; }
        public PokemonService Service { get; private set; }

        private SqliteConnection _connection;

        public PokemonServiceTestFixture()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;
            
            Context = new AppDbContext(options);
            Context.Database.EnsureCreated();

            ClientMock = new Mock<IPokeApiClient>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            Mapper = config.CreateMapper();

            Service = new PokemonService(Context, ClientMock.Object, Mapper);
        }

        public void Dispose()
        {
            Context?.Dispose();
            _connection?.Dispose();
        }
    }
}