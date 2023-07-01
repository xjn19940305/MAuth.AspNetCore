using MAuth.AspNetCore.Api.Swaggers;
using MAuth.AspNetCore.Database.Entities;
using MAuth.AspNetCore.Models.Articles;
using MAuth.AspNetCore.Models.Common;
using MAuth.AspNetCore.MySql;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MAuth.AspNetCore.Api.Controllers.Management
{
    /// <summary>
    /// 文章管理
    /// </summary>
    [ApiController]
    [Route("api/management/[controller]")]
    [ApiGroup(ApiGroupNames.MANAGEMENT)]
    [Authorize]
    public class ArticleController : ControllerBase
    {
        private readonly MAuthDbContext dbContext;

        public ArticleController(
            MAuthDbContext dbContext
            )
        {
            this.dbContext = dbContext;
        }

        /// <summary>
        /// 分页查询文章
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Query([FromQuery] QueryArticleModel request)
        {
            var data = dbContext.Articles.OrderByDescending(x => x.DateCreated).AsNoTracking();
            if (!string.IsNullOrEmpty(request.Author))
            {
                data = data.Where(x => x.Author.Contains(request.Author.Trim()));
            }
            if (!string.IsNullOrEmpty(request.Title))
            {
                data = data.Where(x => x.Title.Contains(request.Title.Trim()));
            }
            var total = data.Count();
            var result = await data.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToListAsync();
            return Ok(new PagedViewModel<dynamic>
            {
                Data = result,
                TotalElements = total
            });
        }
        /// <summary>
        /// 获取文章详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            return Ok(await dbContext.Articles.FirstOrDefaultAsync(x => x.Id == id));
        }

        /// <summary>
        /// 创建文章
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateArtileModel model)
        {
            dbContext.Add(new Article
            {
                Author = model.Author,
                Content = model.Content,
                CategoryId = model.CategoryId,
                Picture = model.Picture,
                Title = model.Title,
            });
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// 修改文章
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> Modify([FromBody] ModifyArticleModel model)
        {
            await dbContext.Articles.Where(x => x.Id == model.Id).ExecuteUpdateAsync(x => x
            .SetProperty(x => x.Author, model.Author)
            .SetProperty(x => x.Title, model.Title)
            .SetProperty(x => x.CategoryId, model.CategoryId)
            .SetProperty(x => x.Picture, model.Picture)
            .SetProperty(x => x.Content, model.Content)
            .SetProperty(x => x.DateUpdate, DateTime.UtcNow));
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] string[] ids)
        {
            await dbContext.Articles.Where(x => ids.Contains(x.Id)).ExecuteDeleteAsync();
            return Ok();
        }
    }
}
