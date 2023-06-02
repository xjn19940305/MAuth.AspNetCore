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
using Org.BouncyCastle.Crypto;
using System.Reflection;

namespace MAuth.AspNetCore.Api.Controllers.Management
{
    [ApiController]
    [Route("api/management/[controller]")]
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
                data = data.Where(x => x.NickName.Contains(request.NickName.Trim()));
            if (!string.IsNullOrWhiteSpace(request.UserName))
                data = data.Where(x => x.UserName.Contains(request.UserName.Trim()));
            if (!string.IsNullOrWhiteSpace(request.Phonenumber))
                data = data.Where(x => x.PhoneNumber.Contains(request.Phonenumber.Trim()));
            if (!string.IsNullOrWhiteSpace(request.Email))
                data = data.Where(x => x.Email.Contains(request.Email.Trim()));
            var totalElements = data.Count();
            var result = await data.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToListAsync();
            return Ok(new PagedViewModel<dynamic>
            {
                Data = result,
                TotalElements = totalElements,
            });
        }
        /// <summary>
        /// 根据ID获取用户信息
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        [HttpGet("{UserId}")]
        public async Task<IActionResult> Get([FromRoute] string UserId)
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
        [Authorize]
        [HttpGet("GetUserInfo")]
        public async Task<IActionResult> GetUserInfo()
        {
            var User = await userManager.FindByIdAsync(userManager.GetUserId(HttpContext.User));
            var permission = await (from u in dbContext.UserRoles
                                    join rl in dbContext.Roles on u.RoleId equals rl.Id
                                    join claim in dbContext.RoleClaims on rl.Id equals claim.RoleId
                                    where u.UserId == User.Id
                                    select claim.ClaimValue).Distinct().ToListAsync();
            return Ok(new
            {
                User = new
                {
                    User.Id,
                    User.UserName,
                    User.NickName,
                    User.PhoneNumber,
                    User.Email,
                },
                permission
            });
        }
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
        /// 删除用户
        /// </summary>
        /// <param name="UserIds"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] string[] UserIds)
        {
            foreach (var id in UserIds)
            {
                var user = await userManager.FindByIdAsync(id);
                if (user != null)
                {
                    var result = await userManager.DeleteAsync(user);
                }
            }
            return Ok();
        }


        /// <summary>
        /// 根据用户ID获取绑定的角色
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        [HttpGet("Get/Roles/{UserId}")]
        public async Task<IActionResult> GetUserBindRole([FromRoute] string UserId)
        {
            return Ok(await dbContext.UserRoles.Where(x => x.UserId == UserId).Select(x => x.RoleId).ToArrayAsync());
        }
        /// <summary>
        /// 用户保存绑定角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("Save/Roles")]
        public async Task<IActionResult> SaveRole([FromBody] SaveRoleModel model)
        {
            dbContext.UserRoles.RemoveRange(await dbContext.UserRoles.Where(f => f.UserId == model.UserId).ToListAsync());
            dbContext.AddRange(model.RoleIds.Select(x => new UserRole { UserId = model.UserId, RoleId = x }));
            await dbContext.SaveChangesAsync();
            return Ok();
        }





    }
}
