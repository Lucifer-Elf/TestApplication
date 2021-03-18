using Servize.Utility.QueryFilter.StringToExpression;
using System;
using System.Linq;

namespace Servize.Utility.QueryFilter
{
    /// <summary>
    /// This is the QueryableExtensions class. This class provides functionality for filtering and pagination.
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// This function apply query filter to parent query.
        /// </summary>
        /// <typeparam name="T">Specify the parameter type for the IQueryable</typeparam>
        /// <param name="parentQuery">Specify the query on which filtering and pagination would be applied</param>
        /// <param name="query">Specify the filter and pagination option provider query</param>
        /// <returns></returns>
        public static IQueryable<T> ApplyQuery<T>(this IQueryable<T> parentQuery, Query query)
        {
            if (query == null)
            {
                return parentQuery;
            }

            return parentQuery
                .ApplyFilter(query.Filter)
                .ApplyOrder(query.SortBy)
                .ApplySkip(query.Offset, query.Limit);
        }

        /// <summary>
        /// This function applies order by sorting to query
        /// </summary>
        /// <typeparam name="T">Specify the parameter type for the IQueryable</typeparam>
        /// <param name="query">Specify the query on which order by will be applied</param>
        /// <param name="order">Specify the order by string</param>
        /// <returns></returns>
        public static IQueryable<T> ApplyOrder<T>(this IQueryable<T> query, string order)
        {
            if (string.IsNullOrEmpty(order))
            {
                return query;
            }

            try
            {
                var compiledOrdering = OrderByParser.Parse(order);
                return compiledOrdering.Apply(query);
            }
            catch (Exception e)
            {
                throw new FormatException($"Provided sort expression '{order}' has incorrect format", e);
            }
        }

        /// <summary>
        /// This function applies filter to query
        /// </summary>
        /// <typeparam name="T">Specify the parameter type for the IQueryable</typeparam>
        /// <param name="query">Specify the query on which filter will be applied</param>
        /// <param name="filter">Specify the filter string</param>
        /// <returns></returns>
        public static IQueryable<T> ApplyFilter<T>(this IQueryable<T> query, string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return query;
            }

            try
            {
                string processedString = QueryFilterUtility.PreprocessFilterString(filter);
                var compiledFilter = new StringToExpressionExtension().Parse<T>(processedString);

                return query.Where(compiledFilter);
            }
            catch (Exception e)
            {
                throw new FormatException($"Provided filter expression '{filter}' has incorrect format", e);
            }
        }

        /// <summary>
        /// This function applies skip and take pagination option to query
        /// </summary>
        /// <typeparam name="T">Specify the parameter type for the IQueryable</typeparam>
        /// <param name="query">Specify the query on which skip and take by will be applied</param>
        /// <param name="skip">Specify the Offset in result data (skip number of entries on top)</param>
        /// <param name="take">Specify the Maximum number of results</param>
        /// <returns></returns>
        public static IQueryable<T> ApplySkip<T>(this IQueryable<T> query, int? skip, int? take)
            => query
                .SkipIf(skip.HasValue, (int)skip.GetValueOrDefault())
                .TakeIf(take.HasValue && take.Value > 0, (int)take.GetValueOrDefault());

        private static IQueryable<T> SkipIf<T>(this IQueryable<T> query, bool predicate, int skip)
            => predicate ? query.Skip(skip) : query;

        private static IQueryable<T> TakeIf<T>(this IQueryable<T> query, bool predicate, int take)
            => predicate ? query.Take(take) : query;
    }
}
