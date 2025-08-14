namespace songs_api_backend.Models.Dtos
{
    /// <summary>
    /// Lightweight song representation for API responses.
    /// </summary>
    // PUBLIC_INTERFACE
    public class SongDto
    {
        // PUBLIC_INTERFACE
        public int Id { get; set; }
        // PUBLIC_INTERFACE
        public string Title { get; set; } = string.Empty;
        // PUBLIC_INTERFACE
        public int DurationSeconds { get; set; }
        // PUBLIC_INTERFACE
        public int ArtistId { get; set; }
        // PUBLIC_INTERFACE
        public string ArtistName { get; set; } = string.Empty;
        // PUBLIC_INTERFACE
        public int GenreId { get; set; }
        // PUBLIC_INTERFACE
        public string GenreName { get; set; } = string.Empty;
    }
}
