using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAuth.AspNetCore.Models.Common
{
    public class PagedViewModel<T> where T : class
    {
        public T Data { get; set; }

        public int TotalElements { get; set; }
    }
}
