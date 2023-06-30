using MAuth.AspNetCore.Api.Swaggers;
using MAuth.AspNetCore.Models.Common;
using MAuth.AspNetCore.MySql;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MAuth.AspNetCore.Api.Controllers
{
    /// <summary>
    /// 轮播图
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [ApiGroup(ApiGroupNames.USER)]
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
