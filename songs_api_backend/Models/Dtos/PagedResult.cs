namespace songs_api_backend.Models.Dtos
{
    /// <summary>
    /// Represents a paged result with total counts for client-side pagination.
    /// </summary>
    // PUBLIC_INTERFACE
    public class PagedResult<T>
    {
        /// <summary>
        /// The items returned for the current page.
        /// </summary>
        // PUBLIC_INTERFACE
        public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();

        /// <summary>
        /// The total number of items across all pages.
        /// </summary>
        // PUBLIC_INTERFACE
        public int Total { get; set; }

        /// <summary>
        /// The current page (1-based).
        /// </summary>
        // PUBLIC_INTERFACE
        public int Page { get; set; }

        /// <summary>
        /// The page size requested.
        /// </summary>
        // PUBLIC_INTERFACE
        public int PageSize { get; set; }
    }
}
