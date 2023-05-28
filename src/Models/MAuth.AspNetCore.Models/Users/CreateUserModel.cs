using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAuth.AspNetCore.Models.Users
{
    public class CreateUserModel
    {
        public string UserName { get; set; }

        public string? NickName { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }
        public string? Password { get; set; }
    }
    public class ModifyUserModel : CreateUserModel
    {
        public string Id { get; set; }
    }
}
