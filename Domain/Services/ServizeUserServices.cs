using AutoMapper;
using Servize.Domain.Repositories;
using Servize.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servize.Domain.Services
{
    public class ServizeUserServices
    {
        private readonly ServizeUserRepository _respository;
        private readonly IMapper _mapper;
        private readonly Utilities _utility;

        public ServizeUserServices(ServizeDBContext dbcontext,
            IMapper mapper, Utilities utility)
        {
            _respository = new ServizeUserRepository(dbcontext);
            _mapper = mapper;
            _utility = utility;

        }

    }
}
