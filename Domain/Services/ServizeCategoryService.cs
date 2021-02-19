using AutoMapper;
using Servize.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servize.Domain.Services
{
    public class ServizeCategoryService
    {
        //private readonly ServizeProviderRespository _respository;
        private readonly IMapper _mapper;
        private readonly Utilities _utility;

        public ServizeCategoryService(ServizeDBContext dbcontext,
            IMapper mapper, Utilities utility)
        {
            //_respository = new ServizeProviderRespository(dbcontext);
            _mapper = mapper;
            _utility = utility;

        }
      
    }
}
