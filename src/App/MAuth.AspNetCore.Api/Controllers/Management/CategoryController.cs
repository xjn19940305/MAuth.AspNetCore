using MAuth.AspNetCore.Api.Swaggers;
using MAuth.AspNetCore.Database.Entities;
using MAuth.AspNetCore.Models.Categories;
using MAuth.AspNetCore.MySql;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MAuth.AspNetCore.Api.Controllers.Management
{
    /// <summary>
    /// 分类管理
    /// </summary>
    [ApiController]
    [Route("api/management/[controller]")]
    [ApiGroup(ApiGroupNames.MANAGEMENT)]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly MAuthDbContext dbContext;

        public CategoryController(
            MAuthDbContext dbContext
            )
        {
            this.dbContext = dbContext;
        }
        /// <summary>
        /// 创建分类
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryModel model)
        {
            await dbContext.AddAsync(new Category
            {
                Name = model.Name,
                ParentId = model.ParentId,
                Sort = model.Sort,
                Type = model.Type,
                Picture = model.Picture,
            });
            await dbContext.SaveChangesAsync();
            return Ok();
        }
        /// <summary>
        /// 修改分类
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> Modify([FromBody] ModifyCategoryModel model)
        {
            await dbContext.Categories.Where(x => x.Id == model.Id).ExecuteUpdateAsync(x => x
            .SetProperty(x => x.Name, model.Name)
            .SetProperty(x => x.Sort, model.Sort)
            .SetProperty(x => x.ParentId, model.ParentId)
            .SetProperty(x => x.Type, model.Type)
            .SetProperty(x => x.Picture, model.Picture)
            .SetProperty(x => x.DateUpdate, DateTime.UtcNow));
            return Ok();
        }
        /// <summary>
        /// 批量删除分类
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] string[] ids)
        {
            await dbContext.Categories.Where(x => ids.Contains(x.Id)).ExecuteDeleteAsync();
            return Ok();
        }
        /// <summary>
        /// 获取所有分类
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var data = await dbContext.Categories.OrderBy(x => x.Sort).ToArrayAsync();
            return Ok(data);
        }
        /// <summary>
        /// 根据ID获取分类
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("{Id}")]
        public async Task<IActionResult> Get([FromRoute] string Id)
        {
            var data = await dbContext.Categories.Where(x => x.Id == Id).FirstOrDefaultAsync();
            return Ok(data);
        }
    }
}
