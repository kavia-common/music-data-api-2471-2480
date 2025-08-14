using Microsoft.EntityFrameworkCore;
using songs_api_backend.Models;

namespace songs_api_backend.Data
{
    /// <summary>
    /// Initializes and seeds the database with sample data when empty.
    /// </summary>
    public static class DbInitializer
    {
        /// <summary>
        /// Ensures the database is created and contains seed data.
        /// </summary>
        /// <param name="db">The MusicDbContext instance.</param>
        /// <param name="logger">Logger for output.</param>
        // PUBLIC_INTERFACE
        public static async Task InitializeAsync(MusicDbContext db, ILogger logger)
        {
            // Create the database if it doesn't exist.
            await db.Database.EnsureCreatedAsync();

            if (await db.Artists.AnyAsync() || await db.Genres.AnyAsync() || await db.Songs.AnyAsync())
            {
                logger.LogInformation("Database already contains data. Skipping seeding.");
                return;
            }

            logger.LogInformation("Seeding initial music data...");

            var artists = new List<Artist>
            {
                new Artist { Name = "The Synth Lords" },
                new Artist { Name = "Acoustic Breeze" },
                new Artist { Name = "Jazz Collective" }
            };

            var genres = new List<Genre>
            {
                new Genre { Name = "Electronic" },
                new Genre { Name = "Acoustic" },
                new Genre { Name = "Jazz" }
            };

            await db.Artists.AddRangeAsync(artists);
            await db.Genres.AddRangeAsync(genres);
            await db.SaveChangesAsync();

            var songs = new List<Song>
            {
                new Song { Title = "Neon Skyline", DurationSeconds = 210, ArtistId = artists[0].Id, GenreId = genres[0].Id },
                new Song { Title = "Circuit Dreams", DurationSeconds = 185, ArtistId = artists[0].Id, GenreId = genres[0].Id },
                new Song { Title = "Wood & Strings", DurationSeconds = 240, ArtistId = artists[1].Id, GenreId = genres[1].Id },
                new Song { Title = "Morning Dew", DurationSeconds = 195, ArtistId = artists[1].Id, GenreId = genres[1].Id },
                new Song { Title = "Blue Note Nights", DurationSeconds = 265, ArtistId = artists[2].Id, GenreId = genres[2].Id }
            };

            await db.Songs.AddRangeAsync(songs);
            await db.SaveChangesAsync();

            logger.LogInformation("Seeding complete.");
        }
    }
}
