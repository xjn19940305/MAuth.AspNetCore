using MAuth.AspNetCore.Api.Swaggers;
using MAuth.AspNetCore.Database.Entities;
using MAuth.AspNetCore.Models.Carousels;
using MAuth.AspNetCore.Models.Common;
using MAuth.AspNetCore.MySql;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MAuth.AspNetCore.Api.Controllers.Management
{
    /// <summary>
    /// 轮播图管理
    /// </summary>
    [ApiController]
    [Route("api/management/[controller]")]
    [ApiGroup(ApiGroupNames.MANAGEMENT)]
    [Authorize]
    public class CarouselController : ControllerBase
    {
        private readonly MAuthDbContext dbContext;

        public CarouselController(
            MAuthDbContext dbContext
            )
        {
            this.dbContext = dbContext;
        }
        /// <summary>
        /// 创建轮播图
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCarouselModel model)
        {
            dbContext.Add(new Carousel
            {
                FilePath = model.FilePath,
                Name = model.Name,
                Sort = model.Sort,
                Link = model.Link
            });
            await dbContext.SaveChangesAsync();
            return Ok();
        }
        /// <summary>
        /// 修改轮播图
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> Modify([FromBody] ModifyCarouselModel model)
        {
            await dbContext.Carousels.Where(x => x.Id == model.Id).ExecuteUpdateAsync(x =>
            x.SetProperty(x => x.Sort, model.Sort)
            .SetProperty(x => x.Name, model.Name)
            .SetProperty(x => x.FilePath, model.FilePath)
            .SetProperty(x => x.Link, model.Link)
            .SetProperty(x => x.DateUpdate, DateTime.UtcNow)
            );
            return Ok();
        }
        /// <summary>
        /// 删除轮播图
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] string[] ids)
        {
            await dbContext.Carousels.Where(x => ids.Contains(x.Id)).ExecuteDeleteAsync();
            return Ok();
        }
        /// <summary>
        /// 查询轮播图
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("{Id}")]
        public async Task<IActionResult> Get([FromRoute] string Id)
        {
            var data = await dbContext.Carousels.Where(x => x.Id == Id).FirstOrDefaultAsync();
            return Ok(data);
        }
        /// <summary>
        /// 查询轮播图列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Query([FromQuery] PagingModel request)
        {
            var data = dbContext.Carousels.OrderBy(x => x.Sort).ThenByDescending(x => x.DateCreated).AsQueryable();
            var total = data.Count();
            var result = await data.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToListAsync();
            return Ok(new PagedViewModel<dynamic>
            {
                TotalElements = total,
                Data = result
            });
        }
    }
}
