using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Servize.Domain.Enums.ServizeEnum;

namespace Servize.Domain.Model
{
    public class ServizeSubCategory
    {
        public int                  Id              { get; set; }

        public string               ServiceName     { get; set; }

        public SubCategories        Category        { get; set; }

        public string               ImageList       { get; set; }

        public DateTime             StartTime       { get; set; }

        public DateTime             EndTime         { get; set; }

        public int                  DaysOFWork      { get; set; }

        public double               AmountCharging  { get; set; }

        public double               UpToDiscount    { get; set; }

        public Area                 Areas           { get; set; }

        public List<ServizeReview>  Reviews      { get; set; }

    }
}
