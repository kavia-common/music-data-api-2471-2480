namespace songs_api_backend.Models
{
    /// <summary>
    /// Represents a musical genre.
    /// </summary>
    // PUBLIC_INTERFACE
    public class Genre
    {
        /// <summary>
        /// Unique identifier of the genre.
        /// </summary>
        // PUBLIC_INTERFACE
        public int Id { get; set; }

        /// <summary>
        /// Display name of the genre.
        /// </summary>
        // PUBLIC_INTERFACE
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Navigation property: Songs in this genre.
        /// </summary>
        public ICollection<Song> Songs { get; set; } = new List<Song>();
    }
}
