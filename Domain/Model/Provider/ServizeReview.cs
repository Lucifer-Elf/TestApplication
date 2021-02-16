using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Servize.Domain.Enums.ServizeEnum;

namespace Servize.Domain.Model.Provider
{
    public class ServizeReview
    {
        [Key]
        public int              Id { get; set; }
        [ForeignKey("ServizeProvider")]
        public int              ProviderId   { get; set; }
        public ServizeProvider ServizeProvider { get; set; }

        public int              HappinessRating { get; set; }

        public SubCategories    SubCategory { get; set; }

        public string           ReviewComment { get; set; }

    }
}
