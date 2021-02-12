using static Servize.Domain.Enums.ServizeEnum;

namespace Servize.Domain.Model
{
    public class ServizeReview
    {
        public int              Id { get; set; }

        public int              Rating { get; set; }

        public SubCategories    SubCategory { get; set; }

        public string           ReviewComment { get; set; }

    }
}
