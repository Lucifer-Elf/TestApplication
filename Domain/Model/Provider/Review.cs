using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Servize.Domain.Enums.ServizeEnum;

namespace Servize.Domain.Model.Provider
{
    public class Review : BaseEntity
    {
        [Key]
        public int              Id { get; set; }
        [ForeignKey("ServizeProvider")]
        public int              ProviderId   { get; set; }
        public Provider ServizeProvider { get; set; }
        public int              HappinessRating { get; set; }
        public string           Product { get; set; }
        public string           ReviewComment { get; set; }

    }
}
