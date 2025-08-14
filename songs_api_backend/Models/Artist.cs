namespace songs_api_backend.Models
{
    /// <summary>
    /// Represents a musical artist.
    /// </summary>
    // PUBLIC_INTERFACE
    public class Artist
    {
        /// <summary>
        /// Unique identifier of the artist.
        /// </summary>
        // PUBLIC_INTERFACE
        public int Id { get; set; }

        /// <summary>
        /// Display name of the artist.
        /// </summary>
        // PUBLIC_INTERFACE
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Navigation property: Songs created by the artist.
        /// </summary>
        public ICollection<Song> Songs { get; set; } = new List<Song>();
    }
}
