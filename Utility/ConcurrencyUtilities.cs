using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Servize.Utility
{
    /// <summary>
    /// This is the ConcurrencyUtilities class. It provides concurrency related utility functions.
    /// </summary>
    public class ConcurrencyUtilities
    {
        /// <summary>
        /// This is the ConcurrencyData structure. It contains status and message.
        /// </summary>
        public struct ConcurrencyData
        {
            /// <summary>
            /// Specifies the concurrency status.
            /// </summary>
            public int Status;

            /// <summary>
            /// Specifies the concurrency message.
            /// </summary>
            public string Message;

            /// <summary>
            /// Constructor for ConcurrencyData.
            /// </summary>
            /// <param name="status">Specifies the concurrency status.</param>
            /// <param name="message">Specifies the concurrency message.</param>
            public ConcurrencyData(int status, string message)
            {
                Status = status;
                Message = message;
            }
        }

        /// <summary>
        /// This function returns ConcurrencyData for concurrency exception.
        /// </summary>
        /// <param name="concurrenyException">Specify DbUpdateConcurrencyException variable.</param>
        /// <returns>The ConcurrencyData.</returns>
        public static ConcurrencyData GetConcurrencyData(DbUpdateConcurrencyException concurrenyException)
        {
            string conflictMessage = "Concurrently modified";
            int status = StatusCodes.Status409Conflict;

            if (concurrenyException != null && concurrenyException.Entries != null && concurrenyException.Entries.Count > 0)
            {
                if (concurrenyException.Entries.First() != null)
                {
                    var entity = concurrenyException.Entries.First().GetDatabaseValues();

                    if (entity == null)
                    {
                        status = StatusCodes.Status410Gone;
                        conflictMessage = "Entry does not exist";
                    }
                }
            }

            return new ConcurrencyData(status, conflictMessage);
        }
    }
}
