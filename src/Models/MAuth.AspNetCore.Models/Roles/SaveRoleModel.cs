using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAuth.AspNetCore.Models.Roles
{
    public class SaveRoleModel
    {
        public string UserId { get; set; }

        public string[] RoleIds { get; set; }
    }
}

