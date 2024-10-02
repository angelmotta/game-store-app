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

    public static GameSummaryDto ToGameSummaryDto(this Game theNewGame) {
        return new GameSummaryDto(
                theNewGame.Id,
                theNewGame.Name,
                theNewGame.Genre!.Name, // validated before
                theNewGame.Price,
                theNewGame.ReleaseDate
        );
    }

    public static GameDetailsDto ToGameDetailsDto(this Game theNewGame) {
        return new GameDetailsDto(
                theNewGame.Id,
                theNewGame.Name,
                theNewGame.GenreId,
                theNewGame.Price,
                theNewGame.ReleaseDate
        );
    }
}
