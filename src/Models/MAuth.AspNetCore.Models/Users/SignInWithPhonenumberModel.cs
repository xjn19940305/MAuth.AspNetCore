using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAuth.AspNetCore.Models.Users
{
    public class SignInWithPhonenumberModel
    {
        //[RegularExpression(@"^\d{11}$",ErrorMessage ="手机号只能为11位数字")]
        //[Required(ErrorMessage ="手机号不能为空")]
        public string Phonenumber { get; set; }

        public string? Code { get; set; }

        public string? OpenId { get; set; }

        public string? Picture { get; set; }

        public string? NickName { get; set; }
    }
}
