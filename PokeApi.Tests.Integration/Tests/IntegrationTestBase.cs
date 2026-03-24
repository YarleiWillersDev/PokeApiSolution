using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using PokeApi.Infrastructure.Data;
using PokeApi.Infrastructure.Integrations;
using PokeApi.Tests.Integration;

namespace PokeApi.Tests.Integration;

public class IntegrationTestBase : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly HttpClient Client;
    private readonly IServiceScope _scope;
    protected readonly AppDbContext Context;

    public IntegrationTestBase(WebApplicationFactory<Program> factory)
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var customFactory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>)
                );

                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseSqlite(connection);
                });
            });
        });

        Client = customFactory.CreateClient();

        _scope = customFactory.Services.CreateScope();
        Context = _scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Context.Database.EnsureCreated();
    }

    protected AppDbContext GetDbContext() => Context;
}