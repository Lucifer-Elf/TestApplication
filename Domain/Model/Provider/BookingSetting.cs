using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using static Servize.Domain.Enums.ServizeEnum;

namespace Servize.Domain.Model.Provider
{
    public class BookingSetting : BaseEntity
    {

        public int Id { get; set; }

        [ForeignKey("ServizeProvider")]
        public int ProviderId { get; set; }
        public Provider ServizeProvider { get; set; }

        public bool BookingProcess { get; set; } // on off current state of acceptance 
        public int SLotsInterval { get; set; }
        public bool MyProperty { get; set; }
        public BookingAssignment BookingAssignment {get;set;}
        public bool AmountMountBasedOnService { get; set; }
        public string NextAvaliablity { get; set; }
    }
}
