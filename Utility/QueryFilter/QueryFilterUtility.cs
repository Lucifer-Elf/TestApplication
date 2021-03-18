namespace Servize.Utility.QueryFilter
{
    /// <summary>
    /// This is the QueryFilterUtility class. It contains utility functions for Query filter.
    /// </summary>
    public class QueryFilterUtility
    {
        /// <summary>
        /// This function process the filter string to handle in and out query
        /// </summary>
        /// <param name="filter">The filter string.</param>
        /// <returns>The processed string.</returns>
        public static string PreprocessFilterString(string filter)
        {
            filter = filter.Replace(">=", "=ge=");
            filter = filter.Replace("<=", "=le=");

            QueryFilterParser parser = new QueryFilterParser
            {
                Filter = filter,
                ProcessedString = filter
            };
            return parser.ParseData(filter);
        }

    }
}
