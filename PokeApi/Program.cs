using Microsoft.EntityFrameworkCore;
using PokeApi.Infrastructure.Data;
using PokeApi.Infrastructure.Integrations;

var builder = WebApplication.CreateBuilder(args);

var ConnectionStringDataBase = builder.Configuration.GetConnectionString("Default");

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(ConnectionStringDataBase));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<IPokeApiClient, PokeApiClient>(client =>
{
    var config = builder.Configuration.GetSection("PokeApi");
    client.BaseAddress = new Uri(config["BaseUrl"]!);
    client.DefaultRequestHeaders.Add("User-Agent", config["UserAgent"]!);
});

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

app.UseHttpsRedirection();
app.Run();

