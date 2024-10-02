using System.Text.Json;
using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Mappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{

    const string GetGameEndpointName = "GetGame";

    // extension method for WebApplication class
    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app) {
        // Define custom behavior for the WebApplication object
        var routerGroup = app.MapGroup("/games")
                            .WithParameterValidation();

        // GET "/games/"
        routerGroup.MapGet("/", async (GameStoreContext dbContext) => {
            var resListGames = await dbContext.Games
                            .Include(game => game.Genre)
                            .Select(game => game.ToGameSummaryDto())
                            .AsNoTracking()
                            .ToListAsync();
            
            return Results.Ok(resListGames);
        }
            
        );

        // GET /games/<id>
        routerGroup.MapGet("/{id}", async (int id, GameStoreContext dbContext) => {
            
            Game? gameResult = await dbContext.Games.FindAsync(id);

            return gameResult is null ? Results.NotFound() : Results.Ok(gameResult.ToGameDetailsDto());
        })
        .WithName(GetGameEndpointName);

        // POST /games/
        routerGroup.MapPost("/", async (CreateGameDto newGameRequest, GameStoreContext dbContext) => {
            if (newGameRequest.GenreId == null) {
                // Handle the case where GenreId is null, though this should not happen due to earlier validation
                throw new InvalidOperationException("GenreId cannot be null");
            }   

            Genre? theGenre = await dbContext.Genre.FindAsync(newGameRequest.GenreId!.Value);
            if (theGenre == null) {
                return Results.BadRequest();
            }
            // theGenre in the request is Valid
            Game theNewGame = newGameRequest.ToEntity();
            theNewGame.Genre = theGenre;

            // Console.WriteLine("------ new Game resource -----");
            // Console.WriteLine($"Name: {theNewGame.Name}, Genre: {theNewGame.Genre?.Name}, GenreId: {theNewGame.GenreId}, Price: {theNewGame.Price}, ReleaseDate: {theNewGame.ReleaseDate}");
            
            dbContext.Games.Add(theNewGame);
            await dbContext.SaveChangesAsync(); // mutate theNewGame Entity Object and include db Id
            
            // Make response to the client
            GameDetailsDto responseSuccess = theNewGame.ToGameDetailsDto();

            return Results.CreatedAtRoute(GetGameEndpointName, new {id = theNewGame.Id}, responseSuccess);
        });

        // PUT /games/<id>
        routerGroup.MapPut("/{id}", async (int id, UpdateGameDto updatedGameRequest, GameStoreContext dbContext) => {
            Game? existingGame = await dbContext.Games.FindAsync(id);

            if (existingGame is null) {
                return Results.NotFound();
            }

            Game updatedGameEntity = updatedGameRequest.ToEntity(id);
            dbContext.Entry(existingGame).CurrentValues.SetValues(updatedGameEntity);
            
            await dbContext.SaveChangesAsync();

            return Results.NoContent(); // HTTP 204 response
        });


        routerGroup.MapDelete("/{id}", async (int id, GameStoreContext dbContext) => {
            var numsRowsDeleted = await dbContext.Games
                        .Where(game => game.Id == id)
                        .ExecuteDeleteAsync();

            if (numsRowsDeleted == 0) {
                return Results.NotFound();
            }

            return Results.NoContent(); // HTTP 204 response
        });

        return routerGroup;
    }
}
