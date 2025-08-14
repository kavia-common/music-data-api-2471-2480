namespace songs_api_backend.Models.Dtos
{
    /// <summary>
    /// Lightweight genre representation for API responses.
    /// </summary>
    // PUBLIC_INTERFACE
    public class GenreDto
    {
        // PUBLIC_INTERFACE
        public int Id { get; set; }
        // PUBLIC_INTERFACE
        public string Name { get; set; } = string.Empty;
        // PUBLIC_INTERFACE
        public int SongsCount { get; set; }
    }
}
