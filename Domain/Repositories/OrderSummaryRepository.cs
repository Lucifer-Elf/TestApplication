using Servize.Domain.Model.OrderDetail;
using Servize.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Servize.Domain.Repositories
{
    public class OrderSummaryRepository : BaseRepository<ServizeDBContext>
    {
        private readonly ServizeDBContext _context;
        public OrderSummaryRepository(ServizeDBContext dbcontext) : base(dbcontext)
        {
            _context = dbcontext;
        }

        internal Task<Response<IList<OrderSummary>>> GetAllOrderSummary()
        {
            throw new NotImplementedException();
        }
    }
}
