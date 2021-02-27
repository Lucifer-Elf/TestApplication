using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servize.Domain.Repositories
{
    public class ServizeUserRepository
    {
        private readonly ServizeDBContext _context;
        public ServizeUserRepository(ServizeDBContext dBContext)
        {
            _context = dBContext;
        }
    }
}
