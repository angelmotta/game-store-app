using GameStore.Api.Dtos;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
const string GetGameEndpointName = "GetGame";

List<GameDto> games = [
    new (
        1, 
        "Street Fighter V",
        "Fighting",
        19.99M,
        new DateOnly(2016, 2, 16)),
    new (
        2, 
        "The Witcher 3: Wild Hunt",
        "RPG",
        29.99M,
        new DateOnly(2015, 5, 19)),
    new (
        3, 
        "Super Mario Odyssey",
        "Platform",
        39.99M,
        new DateOnly(2017, 10, 27))
];

app.MapGet("/games", () => games);

app.MapGet("/games/{id}", (int id) => games.Find(game => game.Id == id))
.WithName(GetGameEndpointName);

app.MapPost("/games", (CreateGameDto newGameRequest) => {
    GameDto newGameObj = new(
        games.Count + 1,
        newGameRequest.Name,
        newGameRequest.Genre,
        newGameRequest.Price,
        newGameRequest.ReleaseDate
    );

    games.Add(newGameObj);

    return Results.CreatedAtRoute(GetGameEndpointName, new {id = newGameObj.Id}, newGameObj);
});

app.MapPut("/games/{id}", (int id, UpdateGameDto updatedGameRequest) => {
    var idx = games.FindIndex(gameObj => gameObj.Id == id);
    if (idx == -1) {
        return Results.NotFound();
    }

    games[idx] = new GameDto(
        id,
        updatedGameRequest.Name,
        updatedGameRequest.Genre,
        updatedGameRequest.Price,
        updatedGameRequest.ReleaseDate
    );

    return Results.NoContent(); // HTTP 204 response
});

app.Run();
