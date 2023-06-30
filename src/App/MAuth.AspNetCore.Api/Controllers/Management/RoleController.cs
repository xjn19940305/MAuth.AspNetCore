using Amazon.Auth.AccessControlPolicy;
using MAuth.AspNetCore.Api.Swaggers;
using MAuth.AspNetCore.Database.Entities;
using MAuth.AspNetCore.Models.Common;
using MAuth.AspNetCore.Models.Roles;
using MAuth.AspNetCore.MySql;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MAuth.AspNetCore.Api.Controllers.Management
{
    /// <summary>
    /// 角色管理
    /// </summary>
    [ApiController]
    [Route("api/management/[controller]")]
    [ApiGroup(ApiGroupNames.MANAGEMENT)]
    [Authorize]
    public class RoleController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<Role> roleManager;
        private readonly MAuthDbContext dbContext;

        public RoleController(
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            MAuthDbContext dbContext
            )
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.dbContext = dbContext;
        }

        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(policy: "RoleCreate")]
        public async Task<IActionResult> Create([FromBody] CreateRoleModel model)
        {
            await roleManager.CreateAsync(new Role
            {
                Name = model.Name,
                Description = model.Description,
                Sort = model.Sort,
            });
            return Ok();
        }
        /// <summary>
        /// 修改角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> Modify([FromBody] ModifyRoleModel model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);
            if (role == null) return NotFound();
            role.Sort = model.Sort;
            role.Description = model.Description;
            role.Name = model.Name;
            await roleManager.UpdateAsync(role);
            return Ok();
        }
        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] string[] ids)
        {
            foreach (var id in ids)
            {
                var role = await roleManager.FindByIdAsync(id);
                if (role != null)
                {
                    var result = await roleManager.DeleteAsync(role);
                    if (!result.Succeeded)
                    {

                    }
                }
            }
            return Ok();
        }
        /// <summary>
        /// 查询角色
        /// </summary>
        /// <param name="RoleId"></param>
        /// <returns></returns>
        [HttpGet("{RoleId}")]
        public async Task<IActionResult> Get([FromRoute] string RoleId)
        {
            return Ok(await dbContext.Roles.Where(x => x.Id == RoleId).FirstOrDefaultAsync());
        }
        /// <summary>
        /// 查询角色列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(policy: "RoleQuery")]
        public async Task<IActionResult> Query([FromQuery] RoleQueryModel request)
        {
            var data = dbContext.Roles.OrderBy(x => x.Sort).ThenByDescending(x => x.DateCreated).AsQueryable();
            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                data = data.Where(x => x.Name.Contains(request.Name));
            }
            var totalElements = data.Count();
            var result = await data.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToListAsync();
            return Ok(new PagedViewModel<dynamic>
            {
                Data = result,
                TotalElements = totalElements
            });
        }
        /// <summary>
        /// 根据角色获取权限
        /// </summary>
        /// <param name="RoleId"></param>
        /// <returns></returns>
        [HttpGet("Authorization/Get/{RoleId}")]
        public async Task<IActionResult> GetRoleAuthorization([FromRoute] string RoleId)
        {
            return Ok(await dbContext.RoleClaims.Where(x => x.RoleId == RoleId && x.ClaimType == "permission").Select(x => x.ClaimValue).ToListAsync());
        }
        /// <summary>
        /// 授权给角色
        /// </summary>
        /// <param name="RoleId"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPut("Authorization/{RoleId}")]
        public async Task<IActionResult> Authorization([FromRoute] string RoleId, [FromBody] string[] data)
        {
            var res = await dbContext.RoleClaims.Where(x => x.RoleId == RoleId).ToListAsync();
            dbContext.RoleClaims.RemoveRange(res);
            foreach (var item in data)
            {
                dbContext.RoleClaims.Add(new RoleClaim { RoleId = RoleId, ClaimType = "permission", ClaimValue = item });
            }
            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
