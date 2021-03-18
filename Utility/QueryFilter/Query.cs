namespace Servize.Utility.QueryFilter
{
    /// <summary>
    /// This is the Query class. It provides filtering and pagination option.
    /// </summary>
    public class Query
    {
        /// <summary>
        /// Offset in result data (skip number of entries on top)
        /// </summary>
        public int? Offset { get; set; }

        /// <summary>
        /// Maximum number of results
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// Filter by given RSQL constraint
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        /// Sort results by specified columns
        /// </summary>
        public string SortBy { get; set; }
    }
}
