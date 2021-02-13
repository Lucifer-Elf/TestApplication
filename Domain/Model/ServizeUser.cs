using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Servize.Domain.Model
{
    public class ServizeUser : ServizeBasicRegistration
    {
        [Key]
        public int Id { get; set; }
    }
}
