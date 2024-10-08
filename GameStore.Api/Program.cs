using GameStore.Api.Data;
using GameStore.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration.GetConnectionString("GameStoreDb");
builder.Services.AddSqlite<GameStoreContext>(connString);

var app = builder.Build();

app.MapGamesEndpoints();
app.MapGenresEndpoints();

app.MigrateDb();

app.Run();
