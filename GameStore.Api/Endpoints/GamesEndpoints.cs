using System.Text.Json;
using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;

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

    // extension method for WebApplication class
    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app) {
        // Define custom behavior for the WebApplication object
        var routerGroup = app.MapGroup("/games")
                            .WithParameterValidation();

        // GET "/games/"
        routerGroup.MapGet("/", () => games);

        // GET /games/<id>
        routerGroup.MapGet("/{id}", (int id) => {
            GameDto? result = games.Find(game => game.Id == id);
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
        .WithName(GetGameEndpointName);

        // POST /games/
        routerGroup.MapPost("/", (CreateGameDto newGameRequest, GameStoreContext dbContext) => {
            if (newGameRequest.GenreId == null) {
                // Handle the case where GenreId is null, though this should not happen due to earlier validation
                throw new InvalidOperationException("GenreId cannot be null");
            }

            Game theNewGame = new(){
                Name = newGameRequest.Name,
                Genre = dbContext.Genre.Find(newGameRequest.GenreId!.Value),
                GenreId = newGameRequest.GenreId!.Value,
                Price = newGameRequest.Price,
                ReleaseDate = newGameRequest.ReleaseDate ?? DateOnly.FromDateTime(DateTime.Now)
            };

            if (theNewGame.Genre == null) {
                return Results.BadRequest();
            }

            // Console.WriteLine("------ new Game resource -----");
            // Console.WriteLine($"Name: {theNewGame.Name}, Genre: {theNewGame.Genre?.Name}, GenreId: {theNewGame.GenreId}, Price: {theNewGame.Price}, ReleaseDate: {theNewGame.ReleaseDate}");
            // Console.WriteLine("------- new Game resource ------");
            dbContext.Games.Add(theNewGame);
            dbContext.SaveChanges();
            
            // Make response to the client
            GameDto responseSuccess = new(
                theNewGame.Id,
                theNewGame.Name,
                theNewGame.Genre!.Name, // validated before
                theNewGame.Price,
                theNewGame.ReleaseDate
            );

            return Results.CreatedAtRoute(GetGameEndpointName, new {id = theNewGame.Id}, responseSuccess);
        });

        routerGroup.MapPut("/{id}", (int id, UpdateGameDto updatedGameRequest) => {
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


        routerGroup.MapDelete("/{id}", (int id) => {
            int val = games.RemoveAll((gameObj) => gameObj.Id == id);
            if (val == 0) {
                return Results.NotFound();
            }

            return Results.NoContent(); // HTTP 204 response
        });

        return routerGroup;
    }
}
