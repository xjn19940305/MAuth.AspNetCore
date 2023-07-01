using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAuth.AspNetCore.Models.Articles
{
    public class CreateArtileModel
    {
        public string Title { get; set; }

        public string? Author { get; set; }

        public string? Content { get; set; }

        public string? CategoryId { get; set; }

        public string? Picture { get; set; }
    }

    public class ModifyArticleModel : CreateArtileModel
    {
        public string Id { get; set; }
    }
}
