using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Dtos;

public record class UpdateGameDto(
    [Required][StringLength(40)]string Name,
    [Required][Range(1, int.MaxValue)] int? GenreId,
    [Range(1, 100)]decimal Price,
    DateOnly ReleaseDate
);
