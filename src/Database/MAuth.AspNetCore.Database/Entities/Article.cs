using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MAuth.AspNetCore.Database.Entities
{
    public class Article
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 作者
        /// </summary>
        public string? Author { get; set; }
        /// <summary>
        /// 分类ID
        /// </summary>
        public string? CategoryId { get; set; }
        /// <summary>
        /// 首图
        /// </summary>
        public string? Picture { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string? Content { get; set; }
        /// <summary>
        /// 阅读量
        /// </summary>
        public int? ReadCount { get; set; } = 0;

        public DateTime? DateCreated { get; set; } = DateTime.UtcNow;

        public DateTime? DateUpdate { get; set; }

        public Category? Category { get; set; }
    }
}
