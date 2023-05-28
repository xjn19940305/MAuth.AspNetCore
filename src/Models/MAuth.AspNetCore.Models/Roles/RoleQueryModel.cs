using MAuth.AspNetCore.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAuth.AspNetCore.Models.Roles
{
    public class RoleQueryModel : PagingModel
    {
        public string? Name { get; set; }
    }
}
