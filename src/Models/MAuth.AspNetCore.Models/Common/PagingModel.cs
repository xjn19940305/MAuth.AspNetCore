using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAuth.AspNetCore.Models.Common
{
    public class PagingModel
    {
        public int PageSize { get; set; } = 10;

        public int Page { get; set; } = 1;
    }
}
