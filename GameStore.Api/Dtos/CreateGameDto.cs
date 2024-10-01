using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Dtos;

public record class CreateGameDto(
    [Required][StringLength(40)]string Name,
    [Required][StringLength(20)]string Genre,
    [Range(1, 100)]decimal Price,
    [Required]DateOnly? ReleaseDate
);
