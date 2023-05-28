using MAuth.AspNetCore.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAuth.AspNetCore.Models.Users
{
    public class UserQueryModel : PagingModel
    {
        public string? Phonenumber { get; set; }
        public string? NickName { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
    }
}
