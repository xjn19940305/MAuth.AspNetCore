using MAuth.AspNetCore.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAuth.AspNetCore.Models.Articles
{
    public class QueryArticleModel : PagingModel
    {
        public string? Title { get; set; }

        public string? Author { get; set; }
    }
}
