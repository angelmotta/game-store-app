using System;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;

namespace GameStore.Api.Mappers;

public static class GameMapper
{
    public static Game ToEntity(this CreateGameDto newGameRequest) {
        return new Game() {
                Name = newGameRequest.Name,
                GenreId = newGameRequest.GenreId!.Value,
                Price = newGameRequest.Price,
                ReleaseDate = newGameRequest.ReleaseDate ?? DateOnly.FromDateTime(DateTime.Now)
        };
    }

    public static GameDto ToDTO(this Game theNewGame) {
        return new GameDto(
                theNewGame.Id,
                theNewGame.Name,
                theNewGame.Genre!.Name, // validated before
                theNewGame.Price,
                theNewGame.ReleaseDate
        );
    }
}
