using GameStore.Api.Data;
using GameStore.Api.Mappers;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GenresEndpoints
{
    public static RouteGroupBuilder MapGenresEndpoints(this WebApplication app) {
        var routerGroup = app
                            .MapGroup("/genres")
                            .WithParameterValidation();


        routerGroup.MapGet("/", async (GameStoreContext dbContext) => {
            var resListGenres = await dbContext.Genre
                                                .Select(genre => genre.ToDto())
                                                .AsNoTracking()
                                                .ToListAsync();
            return Results.Ok(resListGenres);
        });

        return routerGroup;
    }
}
