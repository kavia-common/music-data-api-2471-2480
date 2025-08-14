using Microsoft.EntityFrameworkCore;
using songs_api_backend.Models;

namespace songs_api_backend.Data
{
    /// <summary>
    /// EF Core database context for the music domain.
    /// </summary>
    // PUBLIC_INTERFACE
    public class MusicDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the MusicDbContext.
        /// </summary>
        /// <param name="options">The DbContext options.</param>
        // PUBLIC_INTERFACE
        public MusicDbContext(DbContextOptions<MusicDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Songs table.
        /// </summary>
        // PUBLIC_INTERFACE
        public DbSet<Song> Songs => Set<Song>();

        /// <summary>
        /// Artists table.
        /// </summary>
        // PUBLIC_INTERFACE
        public DbSet<Artist> Artists => Set<Artist>();

        /// <summary>
        /// Genres table.
        /// </summary>
        // PUBLIC_INTERFACE
        public DbSet<Genre> Genres => Set<Genre>();

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Artist>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Name).IsRequired().HasMaxLength(200);
                entity.HasIndex(a => a.Name).HasDatabaseName("IX_Artists_Name");
            });

            modelBuilder.Entity<Genre>(entity =>
            {
                entity.HasKey(g => g.Id);
                entity.Property(g => g.Name).IsRequired().HasMaxLength(100);
                entity.HasIndex(g => g.Name).HasDatabaseName("IX_Genres_Name");
            });

            modelBuilder.Entity<Song>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Title).IsRequired().HasMaxLength(300);
                entity.Property(s => s.DurationSeconds).HasDefaultValue(0);

                entity.HasOne(s => s.Artist)
                      .WithMany(a => a.Songs)
                      .HasForeignKey(s => s.ArtistId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.Genre)
                      .WithMany(g => g.Songs)
                      .HasForeignKey(s => s.GenreId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(s => s.Title).HasDatabaseName("IX_Songs_Title");
                entity.HasIndex(s => new { s.ArtistId, s.GenreId }).HasDatabaseName("IX_Songs_Artist_Genre");
            });
        }
    }
}
