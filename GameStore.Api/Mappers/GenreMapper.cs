using GameStore.Api.Dtos;
using GameStore.Api.Entities;

namespace GameStore.Api.Mappers;

public static class GenreMapper
{
    public static GenreDto ToDto(this Genre genre) {
        return new GenreDto(genre.Id, genre.Name);
    }
}
