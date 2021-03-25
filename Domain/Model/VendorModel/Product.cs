using System;
using System.ComponentModel.DataAnnotations.Schema;
using static Servize.Domain.Enums.ServizeEnum;

namespace Servize.Domain.Model.VendorModel
{
    public class Product :BaseEntity
    {
        public int Id { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Quantity { get; set; }
        public decimal VAT { get; set; }
        public decimal UnitPrice { get; set; }
        public string Discount { get; set; }
        public decimal Total { get; set; }
        public string ImageList { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int DaysOFWork { get; set; }
        public double PriceQuote { get; set; }
        public double VariablePrice { get; set; }
        public Area Areas { get; set; }

       


    }
}
