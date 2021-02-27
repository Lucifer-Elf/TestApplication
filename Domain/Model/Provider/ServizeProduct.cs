using System;
using System.ComponentModel.DataAnnotations.Schema;
using static Servize.Domain.Enums.ServizeEnum;

namespace Servize.Domain.Model.Provider
{
    public class ServizeProduct
    {
        public int Id { get; set; }

        [ForeignKey("ServizeCategory")]
        public int ServizeCategoryId { get; set; }
        public ServizeCategory ServizeCategory { get; set; }

        public string ServiceName { get; set; }

        public SubCategories Category { get; set; }

        public string ImageList { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int DaysOFWork { get; set; }

        public double PriceQuote { get; set; }

        public double VariablePrice { get; set; }

        public double Discount { get; set; }

        public Area Areas { get; set; }

    }
}
