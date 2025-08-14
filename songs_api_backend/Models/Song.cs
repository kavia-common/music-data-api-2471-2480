namespace songs_api_backend.Models
{
    /// <summary>
    /// Represents a song track.
    /// </summary>
    // PUBLIC_INTERFACE
    public class Song
    {
        /// <summary>
        /// Unique identifier of the song.
        /// </summary>
        // PUBLIC_INTERFACE
        public int Id { get; set; }

        /// <summary>
        /// Title of the song.
        /// </summary>
        // PUBLIC_INTERFACE
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Duration of the song in seconds.
        /// </summary>
        // PUBLIC_INTERFACE
        public int DurationSeconds { get; set; }

        /// <summary>
        /// Foreign key referencing the artist.
        /// </summary>
        // PUBLIC_INTERFACE
        public int ArtistId { get; set; }

        /// <summary>
        /// Foreign key referencing the genre.
        /// </summary>
        // PUBLIC_INTERFACE
        public int GenreId { get; set; }

        /// <summary>
        /// Navigation property: the artist of this song.
        /// </summary>
        public Artist? Artist { get; set; }

        /// <summary>
        /// Navigation property: the genre of this song.
        /// </summary>
        public Genre? Genre { get; set; }
    }
}
