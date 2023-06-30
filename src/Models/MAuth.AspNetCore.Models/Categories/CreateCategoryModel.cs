using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAuth.AspNetCore.Models.Categories
{
    public class CreateCategoryModel
    {
        public string Name { get; set; }

        public int? Sort { get; set; }

        public string ParentId { get; set; }

        public string? Picture { get; set; }
        /// <summary>
        /// 0单页 1QA
        /// </summary>
        public int? Type { get; set; }
    }

    public class ModifyCategoryModel : CreateCategoryModel
    {
        public string Id { get; set; }
    }
}
