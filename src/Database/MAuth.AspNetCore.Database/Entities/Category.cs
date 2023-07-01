using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAuth.AspNetCore.Database.Entities
{
    public class Category
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Name { get; set; }

        public int? Sort { get; set; }

        public string? ParentId { get; set; }
        /// <summary>
        /// 待商榷
        /// </summary>
        public int? Type { get; set; }
        /// <summary>
        /// 分类LOGO图
        /// </summary>
        public string? Picture { get; set; }

        public DateTime? DateCreated { get; set; } = DateTime.UtcNow;

        public DateTime? DateUpdate { get; set; }

        public virtual ICollection<Article> Articles { get; set; } = new HashSet<Article>();
    }
}
