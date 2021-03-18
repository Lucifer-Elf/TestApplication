using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Servize.Utility.QueryFilter
{
    /// <summary>
    /// This is the OrderByParser class. It will handle order by sorting.
    /// </summary>
    public static class OrderByParser
    {
        /// <summary>
        /// This is the Parse function. It will parse the order by string.
        /// </summary>
        /// <param name="orderBy">Specify the order by string</param>
        /// <returns></returns>
        public static OrderToken Parse(string orderBy)
        {
            if (string.IsNullOrEmpty(orderBy)) throw new ArgumentNullException(nameof(orderBy));

            var tokens = orderBy.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                .Select(segment => new OrderToken(segment)).ToArray();

            var orderToken = tokens[0];
            tokens.Skip(1).Aggregate(orderToken, (token, next) => token.Next = next);

            return orderToken;
        }
    }

    /// <summary>
    /// This is the OrderToken class.
    /// </summary>
    public class OrderToken
    {
        private readonly string _propertyPath;
        private readonly SortOrder _order;

        /// <summary>
        /// This is the OrderToken property
        /// </summary>
        public OrderToken Next { get; set; }

        /// <summary>
        /// Constructor for OrderToken class.
        /// </summary>
        /// <param name="segment">Specify the string segment of the sort by query</param>
        public OrderToken(string segment)
        {
            List<string> parts;
            if (segment.StartsWith("-"))
            {
                parts = new List<string>
                {
                    segment.Remove(0, 1),
                    "Desc"
                };
            }
            else
            {
                string column = segment;
                if (segment.StartsWith("+"))
                    column = segment.Remove(0, 1);

                parts = new List<string>
                {
                    column,
                    "Asc"
                };
            }

            if (parts == null || parts.Count < 1 || parts.Count > 2)
                throw new ArgumentException($"Segment '{segment}' has incorrect format");

            _propertyPath = parts[0];
            _order = parts.Count == 2 ? (SortOrder)Enum.Parse(typeof(SortOrder), parts[1]) : SortOrder.Asc;
        }

        /// <summary>
        /// This function applied the order by clause to the query.
        /// </summary>
        /// <typeparam name="T">Specify the parameter type for the IQueryable</typeparam>
        /// <param name="query">Specify the query on which order by will be applied</param>
        /// <param name="firstCall">Specify if its the first call</param>
        /// <returns></returns>
        public IQueryable<T> Apply<T>(IQueryable<T> query, bool firstCall = true)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var memberExpression =
                (MemberExpression)_propertyPath.Split('.').Aggregate((Expression)parameter, Expression.Property);

            var call = Expression.Call(
                typeof(Queryable), ChooseMethod(), new[]
                {
                    typeof(T),
                    ((PropertyInfo) memberExpression.Member).PropertyType
                },
                query.Expression,
                Expression.Lambda(memberExpression, parameter));

            var ordered = (IOrderedQueryable<T>)query.Provider.CreateQuery<T>(call);

            return Next?.Apply<T>(ordered, false) ?? ordered;

            string ChooseMethod()
            {
                switch (_order)
                {
                    case SortOrder.Asc: return firstCall ? "OrderBy" : "ThenBy";
                    case SortOrder.Desc: return firstCall ? "OrderByDescending" : "ThenByDescending";
                    default:
                        break;
                }

                throw new NotImplementedException();
            }
        }

        private enum SortOrder
        {
            Asc,
            Desc
        }
    }
}
