using MAuth.AspNetCore.Database.Entities;
using MAuth.AspNetCore.Models.Common;
using MAuth.AspNetCore.Models.Roles;
using MAuth.AspNetCore.Models.Users;
using MAuth.AspNetCore.MySql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace MAuth.AspNetCore.Api.Controllers.Management
{
    [ApiController]
    [Route("api/management/[controller]/[action]")]
    [ApiExplorerSettings(GroupName = "management")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly RoleManager<Role> roleManager;
        private readonly ILogger<UserController> logger;
        private readonly MAuthDbContext dbContext;
        //private readonly IModuleService moduleService;
        private const string EmptyUserSecurityStamp = "e7b51244f3ad4511b9739dfc29b261d5";
        private JwtBearerOptions jwtOptions;
        public UserController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<Role> roleManager,
            ILogger<UserController> logger,
            IOptions<JwtBearerOptions> options,
            MAuthDbContext dbContext
            //IModuleService moduleService
            )
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.logger = logger;
            this.dbContext = dbContext;
            //this.moduleService = moduleService;
            jwtOptions = options.Value;
        }

        /// <summary>
        /// 用户列表查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Query([FromQuery] UserQueryModel request)
        {
            var t = dbContext.Users;
            var data = dbContext.Users
                .Select(x => new
                {
                    x.Id,
                    x.UserName,
                    x.NickName,
                    x.Email,
                    x.PhoneNumber,
                    x.CreateDate
                })
                .OrderByDescending(x => x.CreateDate).AsQueryable();
            if (!string.IsNullOrWhiteSpace(request.NickName))
                data = data.Where(x => x.NickName.Contains(request.NickName));
            if (!string.IsNullOrWhiteSpace(request.UserName))
                data = data.Where(x => x.UserName.Contains(request.UserName));
            var totalElements = data.Count();
            var result = await data.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToListAsync();
            return Ok(new PagedViewModel<dynamic>
            {
                Data = result,
                TotalElements = totalElements,
            });
        }

        //[Authorize]
        //[HttpGet]
        //public async Task<dynamic> GetAdminSigninInfo()
        //{
        //    var User = await userManager.FindByIdAsync(userManager.GetUserId(HttpContext.User));
        //    var Role = await userManager.GetRolesAsync(User);
        //    var roleIds = await dbContext.Roles.Where(x => Role.Contains(x.Name)).Select(x => new { x.Id, Role = x.Name }).ToListAsync();
        //    var Module = await dbContext.RoleModules
        //        .Where(x => roleIds.Select(g => g.Id).Contains(x.RoleId))
        //        .Select(x => x.Module).Where(x => x.Type == Marketing.Model.Enums.ModuleTypeEnum.MODULE)
        //        .OrderBy(x => x.Sort).ToListAsync();
        //    var route = new RouterModel()
        //    {
        //        children = new List<RouterModel>(),
        //        router = "root"
        //    };
        //    var RootList = Module.Where(x => x.ParentId == "0").ToList();
        //    foreach (var m in RootList)
        //    {
        //        var model = new RouterModel
        //        {
        //            router = m.Component,
        //            path = m.Path,
        //            name = m.Name,
        //            icon = m.Icon,
        //            invisible = m.Invisible
        //        };
        //        RecursionRoute(Module, m, model);
        //        route.children.Add(model);
        //    }
        //    return new
        //    {
        //        User = new
        //        {
        //            User.Id,
        //            User.Email,
        //            User.IsDelete,
        //            User.UserName,
        //            User.PhoneNumber,
        //            User.Picture
        //        },
        //        Role = roleIds,
        //        route
        //    };
        //}
        //private void RecursionRoute(List<Module> list, Module module, RouterModel current)
        //{
        //    var childList = list.Where(x => x.ParentId == module.Id).ToList();
        //    foreach (var item in childList)
        //    {
        //        if (current.children == null)
        //            current.children = new List<RouterModel>();
        //        var model = new RouterModel
        //        {
        //            router = item.Component,
        //            path = item.Path,
        //            name = item.Name,
        //            icon = item.Icon,
        //            invisible = item.Invisible
        //        };
        //        RecursionRoute(list, item, model);
        //        current.children.Add(model);
        //    }
        //}
        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserModel model)
        {
            var user = new User()
            {
                Email = model.Email,
                UserName = model.UserName,
                PhoneNumber = model.PhoneNumber,
                NickName = model.NickName
            };
            if (!string.IsNullOrEmpty(model.Password))
            {
                await userManager.AddPasswordAsync(user, model.Password);
            }
            await dbContext.SaveChangesAsync();
            var result = await userManager.CreateAsync(user);
            if (result.Succeeded)
                return Ok();
            return BadRequest(result.Errors.FirstOrDefault());
        }
        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> ModifyUser([FromBody] ModifyUserModel model)
        {
            var user = await dbContext.Users.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
            user.PhoneNumber = model.PhoneNumber;
            user.NickName = model.NickName;
            user.Email = model.Email;
            user.UserName = model.UserName;
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                await userManager.ResetPasswordAsync(user, token, model.Password);
            }
            await userManager.UpdateAsync(user);
            return Ok();
        }
        /// <summary>
        /// 根据ID获取用户信息
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string UserId)
        {
            var data = await dbContext.Users
                .Where(x => x.Id == UserId)
                .Select(x => new
                {
                    x.Id,
                    x.UserName,
                    x.NickName,
                    x.PhoneNumber,
                    x.Email
                }).FirstOrDefaultAsync();
            return Ok(data);
        }

        /// <summary>
        /// 根据用户ID获取绑定的角色
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUserBindRole([FromQuery] string UserId)
        {
            return Ok(await dbContext.UserRoles.Where(x => x.UserId == UserId).Select(x => x.RoleId).ToArrayAsync());
        }
        /// <summary>
        /// 用户保存绑定角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> SaveRole([FromBody] SaveRoleModel model)
        {
            dbContext.UserRoles.RemoveRange(await dbContext.UserRoles.Where(f => f.UserId == model.UserId).ToListAsync());
            dbContext.AddRange(model.RoleIds.Select(x => new UserRole { UserId = model.UserId, RoleId = x }));
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] string[] UserIds)
        {
            dbContext.Users.RemoveRange(UserIds.Select(x => new Database.Entities.User { Id = x }));
            await dbContext.SaveChangesAsync();
            return Ok();
        }



    }
}
