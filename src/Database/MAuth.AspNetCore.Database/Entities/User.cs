using MAuth.AspNetCore.Enum;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MAuth.AspNetCore.Database.Entities
{
    public class User : IdentityUser
    {
        public string? NickName { get; set; }
        /// <summary>
        /// 用户的性别，值为0男性，值1是女性，其他值时是未知
        /// </summary>
        public GenderEnum? Gender { get; set; }
        public string? Avatar { get; set; }
        public string? Country { get; set; }
        public string? Province { get; set; }
        public string? City { get; set; }
        public string? Area { get; set; }
        public string? Address { get; set; }

        public DateTime? BirthDate { get; set; }
        public DateTime? UpdateAt { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// 是否逻辑删除
        /// </summary>
        public bool IsDelete { get; set; }
    }
}
