using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAuth.AspNetCore.Models.Roles
{
    public class CreateRoleModel
    {
        public string Name { get; set; }

        public string? Description { get; set; }
        public int? Sort { get; set; }
    }
}

