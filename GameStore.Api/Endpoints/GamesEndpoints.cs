using GameStore.Api.Dtos;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{

    const string GetGameEndpointName = "GetGame";

    private static List<GameDto> games = [
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

    // extension method
    public static WebApplication MapGamesEndpoints(this WebApplication app) {
        // Define custom behavior for the WebApplication object
        app.MapGet("/games", () => games);

        app.MapGet("/games/{id}", (int id) => {
            GameDto? result = games.Find(game => game.Id == id);
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
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


        app.MapDelete("/games/{id}", (int id) => {
            int val = games.RemoveAll((gameObj) => gameObj.Id == id);
            if (val == 0) {
                return Results.NotFound();
            }

            return Results.NoContent(); // HTTP 204 response
        });

        return app;
    }
}
