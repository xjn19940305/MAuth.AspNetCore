using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAuth.AspNetCore.Models.Carousels
{
    public class CreateCarouselModel
    {
        public string Name { get; set; }

        public string FilePath { get; set; }

        public int? Sort { get; set; }

        public string? Link { get; set; }
    }

    public class ModifyCarouselModel : CreateCarouselModel
    {
        public string Id { get; set; }
    }
}
