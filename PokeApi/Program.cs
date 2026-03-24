using Microsoft.EntityFrameworkCore;
using PokeApi.API.Mapper;
using PokeApi.Application.Service;
using PokeApi.Infrastructure.Data;
using PokeApi.Infrastructure.Integrations;
using PokeApiTeste.Handlers;

var builder = WebApplication.CreateBuilder(args);

var ConnectionStringDataBase = builder.Configuration.GetConnectionString("Default");

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(ConnectionStringDataBase));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<IPokeApiClient, PokeApiClient>(client =>
{
    var config = builder.Configuration.GetSection("PokeApi");
    client.BaseAddress = new Uri(config["BaseUrl"]!);
    client.DefaultRequestHeaders.Add("User-Agent", config["UserAgent"]!);
});

builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped<IPokemonService, PokemonService>();
builder.Services.AddExceptionHandler<UniversalAppExceptionHandler>();
builder.Services.AddExceptionHandler<DefaultHandler>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

app.UseExceptionHandler(errorApp => { 
    errorApp.Run(async context => { });
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program { }
