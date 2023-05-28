using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAuth.AspNetCore.Database.Entities
{
    public class Role : IdentityRole
    {
        public string? Description { get; set; }
        public int? Sort { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public DateTime? UpdateDate { get; set; }
    }
}
