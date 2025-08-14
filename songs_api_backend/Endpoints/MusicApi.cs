using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using songs_api_backend.Data;
using songs_api_backend.Models.Dtos;

namespace songs_api_backend.Endpoints
{
    /// <summary>
    /// Provides endpoint registration for the Music API (songs, artists, genres).
    /// </summary>
    // PUBLIC_INTERFACE
    public static class MusicApi
    {
        /// <summary>
        /// Registers Music API endpoints under the /api path.
        /// </summary>
        /// <param name="endpoints">The endpoint route builder.</param>
        /// <returns>The updated endpoint route builder.</returns>
        // PUBLIC_INTERFACE
        public static IEndpointRouteBuilder MapMusicApi(this IEndpointRouteBuilder endpoints)
        {
            var api = endpoints.MapGroup("/api").WithTags("Music");

            MapSongs(api);
            MapArtists(api);
            MapGenres(api);

            return endpoints;
        }

        private static void MapSongs(IEndpointRouteBuilder api)
        {
            var songs = api.MapGroup("/songs").WithTags("Songs");

            songs.MapGet("/", async Task<Results<Ok<PagedResult<SongDto>>, BadRequest<string>>> (
                [FromServices] MusicDbContext db,
                [FromQuery] string? search,
                [FromQuery] int? artistId,
                [FromQuery] int? genreId,
                [FromQuery] int page = 1,
                [FromQuery] int pageSize = 20) =>
            {
                if (page <= 0 || pageSize <= 0 || pageSize > 200)
                {
                    return TypedResults.BadRequest("Invalid pagination parameters. page >= 1, 1 <= pageSize <= 200");
                }

                var query = db.Songs.AsNoTracking()
                    .Include(s => s.Artist)
                    .Include(s => s.Genre)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    var sLower = search.ToLower();
                    query = query.Where(s => s.Title.ToLower().Contains(sLower));
                }

                if (artistId.HasValue)
                {
                    query = query.Where(s => s.ArtistId == artistId.Value);
                }

                if (genreId.HasValue)
                {
                    query = query.Where(s => s.GenreId == genreId.Value);
                }

                var total = await query.CountAsync();

                var items = await query
                    .OrderBy(s => s.Title)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(s => new SongDto
                    {
                        Id = s.Id,
                        Title = s.Title,
                        DurationSeconds = s.DurationSeconds,
                        ArtistId = s.ArtistId,
                        ArtistName = s.Artist != null ? s.Artist.Name : "",
                        GenreId = s.GenreId,
                        GenreName = s.Genre != null ? s.Genre.Name : ""
                    })
                    .ToListAsync();

                var result = new PagedResult<SongDto>
                {
                    Items = items,
                    Total = total,
                    Page = page,
                    PageSize = pageSize
                };

                return TypedResults.Ok(result);
            })
            .WithName("ListSongs")
            .WithSummary("List songs")
            .WithDescription("Returns a paginated list of songs with optional search by title and filters by artist or genre.")
            .Produces<PagedResult<SongDto>>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest);

            songs.MapGet("/{id:int}", async Task<Results<Ok<SongDto>, NotFound>> (
                [FromServices] MusicDbContext db,
                [FromRoute] int id) =>
            {
                var song = await db.Songs.AsNoTracking()
                    .Include(s => s.Artist)
                    .Include(s => s.Genre)
                    .Where(s => s.Id == id)
                    .Select(s => new SongDto
                    {
                        Id = s.Id,
                        Title = s.Title,
                        DurationSeconds = s.DurationSeconds,
                        ArtistId = s.ArtistId,
                        ArtistName = s.Artist != null ? s.Artist.Name : "",
                        GenreId = s.GenreId,
                        GenreName = s.Genre != null ? s.Genre.Name : ""
                    })
                    .FirstOrDefaultAsync();

                return song is null ? TypedResults.NotFound() : TypedResults.Ok(song);
            })
            .WithName("GetSongById")
            .WithSummary("Get song by ID")
            .WithDescription("Retrieves details for a specific song by its ID.")
            .Produces<SongDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
        }

        private static void MapArtists(IEndpointRouteBuilder api)
        {
            var artists = api.MapGroup("/artists").WithTags("Artists");

            artists.MapGet("/", async Task<Results<Ok<PagedResult<ArtistDto>>, BadRequest<string>>> (
                [FromServices] MusicDbContext db,
                [FromQuery] int page = 1,
                [FromQuery] int pageSize = 20) =>
            {
                if (page <= 0 || pageSize <= 0 || pageSize > 200)
                {
                    return TypedResults.BadRequest("Invalid pagination parameters. page >= 1, 1 <= pageSize <= 200");
                }

                var baseQuery = db.Artists.AsNoTracking();

                var total = await baseQuery.CountAsync();

                var items = await baseQuery
                    .OrderBy(a => a.Name)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(a => new ArtistDto
                    {
                        Id = a.Id,
                        Name = a.Name,
                        SongsCount = a.Songs.Count
                    })
                    .ToListAsync();

                return TypedResults.Ok(new PagedResult<ArtistDto>
                {
                    Items = items,
                    Total = total,
                    Page = page,
                    PageSize = pageSize
                });
            })
            .WithName("ListArtists")
            .WithSummary("List artists")
            .WithDescription("Returns a paginated list of artists.")
            .Produces<PagedResult<ArtistDto>>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest);

            artists.MapGet("/{id:int}", async Task<Results<Ok<ArtistDto>, NotFound>> (
                [FromServices] MusicDbContext db,
                [FromRoute] int id) =>
            {
                var artist = await db.Artists.AsNoTracking()
                    .Where(a => a.Id == id)
                    .Select(a => new ArtistDto
                    {
                        Id = a.Id,
                        Name = a.Name,
                        SongsCount = a.Songs.Count
                    })
                    .FirstOrDefaultAsync();

                return artist is null ? TypedResults.NotFound() : TypedResults.Ok(artist);
            })
            .WithName("GetArtistById")
            .WithSummary("Get artist by ID")
            .WithDescription("Retrieves details for a specific artist by ID.")
            .Produces<ArtistDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

            artists.MapGet("/{id:int}/songs", async Task<Results<Ok<PagedResult<SongDto>>, NotFound, BadRequest<string>>> (
                [FromServices] MusicDbContext db,
                [FromRoute] int id,
                [FromQuery] int page = 1,
                [FromQuery] int pageSize = 20) =>
            {
                var exists = await db.Artists.AsNoTracking().AnyAsync(a => a.Id == id);
                if (!exists) return TypedResults.NotFound();

                if (page <= 0 || pageSize <= 0 || pageSize > 200)
                {
                    return TypedResults.BadRequest("Invalid pagination parameters. page >= 1, 1 <= pageSize <= 200");
                }

                var query = db.Songs.AsNoTracking()
                    .Include(s => s.Artist)
                    .Include(s => s.Genre)
                    .Where(s => s.ArtistId == id);

                var total = await query.CountAsync();

                var items = await query
                    .OrderBy(s => s.Title)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(s => new SongDto
                    {
                        Id = s.Id,
                        Title = s.Title,
                        DurationSeconds = s.DurationSeconds,
                        ArtistId = s.ArtistId,
                        ArtistName = s.Artist != null ? s.Artist.Name : "",
                        GenreId = s.GenreId,
                        GenreName = s.Genre != null ? s.Genre.Name : ""
                    })
                    .ToListAsync();

                return TypedResults.Ok(new PagedResult<SongDto>
                {
                    Items = items,
                    Total = total,
                    Page = page,
                    PageSize = pageSize
                });
            })
            .WithName("ListSongsByArtist")
            .WithSummary("List songs by artist")
            .WithDescription("Lists songs belonging to a specified artist.")
            .Produces<PagedResult<SongDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<string>(StatusCodes.Status400BadRequest);
        }

        private static void MapGenres(IEndpointRouteBuilder api)
        {
            var genres = api.MapGroup("/genres").WithTags("Genres");

            genres.MapGet("/", async Task<Results<Ok<PagedResult<GenreDto>>, BadRequest<string>>> (
                [FromServices] MusicDbContext db,
                [FromQuery] int page = 1,
                [FromQuery] int pageSize = 20) =>
            {
                if (page <= 0 || pageSize <= 0 || pageSize > 200)
                {
                    return TypedResults.BadRequest("Invalid pagination parameters. page >= 1, 1 <= pageSize <= 200");
                }

                var baseQuery = db.Genres.AsNoTracking();

                var total = await baseQuery.CountAsync();

                var items = await baseQuery
                    .OrderBy(g => g.Name)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(g => new GenreDto
                    {
                        Id = g.Id,
                        Name = g.Name,
                        SongsCount = g.Songs.Count
                    })
                    .ToListAsync();

                return TypedResults.Ok(new PagedResult<GenreDto>
                {
                    Items = items,
                    Total = total,
                    Page = page,
                    PageSize = pageSize
                });
            })
            .WithName("ListGenres")
            .WithSummary("List genres")
            .WithDescription("Returns a paginated list of genres.")
            .Produces<PagedResult<GenreDto>>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest);

            genres.MapGet("/{id:int}", async Task<Results<Ok<GenreDto>, NotFound>> (
                [FromServices] MusicDbContext db,
                [FromRoute] int id) =>
            {
                var genre = await db.Genres.AsNoTracking()
                    .Where(g => g.Id == id)
                    .Select(g => new GenreDto
                    {
                        Id = g.Id,
                        Name = g.Name,
                        SongsCount = g.Songs.Count
                    })
                    .FirstOrDefaultAsync();

                return genre is null ? TypedResults.NotFound() : TypedResults.Ok(genre);
            })
            .WithName("GetGenreById")
            .WithSummary("Get genre by ID")
            .WithDescription("Retrieves details for a specific genre by ID.")
            .Produces<GenreDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

            genres.MapGet("/{id:int}/songs", async Task<Results<Ok<PagedResult<SongDto>>, NotFound, BadRequest<string>>> (
                [FromServices] MusicDbContext db,
                [FromRoute] int id,
                [FromQuery] int page = 1,
                [FromQuery] int pageSize = 20) =>
            {
                var exists = await db.Genres.AsNoTracking().AnyAsync(g => g.Id == id);
                if (!exists) return TypedResults.NotFound();

                if (page <= 0 || pageSize <= 0 || pageSize > 200)
                {
                    return TypedResults.BadRequest("Invalid pagination parameters. page >= 1, 1 <= pageSize <= 200");
                }

                var query = db.Songs.AsNoTracking()
                    .Include(s => s.Artist)
                    .Include(s => s.Genre)
                    .Where(s => s.GenreId == id);

                var total = await query.CountAsync();

                var items = await query
                    .OrderBy(s => s.Title)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(s => new SongDto
                    {
                        Id = s.Id,
                        Title = s.Title,
                        DurationSeconds = s.DurationSeconds,
                        ArtistId = s.ArtistId,
                        ArtistName = s.Artist != null ? s.Artist.Name : "",
                        GenreId = s.GenreId,
                        GenreName = s.Genre != null ? s.Genre.Name : ""
                    })
                    .ToListAsync();

                return TypedResults.Ok(new PagedResult<SongDto>
                {
                    Items = items,
                    Total = total,
                    Page = page,
                    PageSize = pageSize
                });
            })
            .WithName("ListSongsByGenre")
            .WithSummary("List songs by genre")
            .WithDescription("Lists songs belonging to a specified genre.")
            .Produces<PagedResult<SongDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<string>(StatusCodes.Status400BadRequest);
        }
    }
}
