using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAuth.AspNetCore.Models.Options
{
    public class MongoOptions
    {
        public string Connection { get; set; }
        public string Prefix { get; set; }
    }
}
