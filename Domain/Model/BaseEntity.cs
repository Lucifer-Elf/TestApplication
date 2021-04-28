using System;
using System.ComponentModel.DataAnnotations;

namespace Servize.Domain.Model
{
    public class BaseEntity
    {
        [ConcurrencyCheck]
        public DateTime? Modified { get; set; }
    }
}
