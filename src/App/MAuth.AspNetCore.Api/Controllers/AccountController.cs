using IdentityModel;
using MAuth.AspNetCore.Api.Swaggers;
using MAuth.AspNetCore.Database.Entities;
using MAuth.AspNetCore.Models.Users;
using MAuth.AspNetCore.MySql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MAuth.AspNetCore.Api.Controllers
{
    /// <summary>
    /// 登录
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [ApiGroup(ApiGroupNames.USER)]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly IConfiguration configuration;
        private readonly MAuthDbContext dbContext;
        private const string EmptyUserSecurityStamp = "e7b51244f3ad4511b9739dfc29b261d5";
        private JwtBearerOptions jwtOptions;
        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration,
            IOptions<JwtBearerOptions> options,
            MAuthDbContext dbContext
            )
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.dbContext = dbContext;
            jwtOptions = options.Value;
        }
        /// <summary>
        /// 账号密码登录
        /// </summary>
        /// <returns></returns>
        [HttpPost("SignInWithPassword")]
        public async Task<IActionResult> SignInWithPassword([FromBody] SiginInWithPasswordModel request)
        {
            var user = await userManager.FindByNameAsync(request.UserName);
            if (user == null) { return BadRequest("用户名不存在!"); }
            var result = await signInManager.PasswordSignInAsync(user, request.Password, true, false);
            if (result.Succeeded)
            {
                await signInManager.SignInAsync(user, true);
                return Ok();
            }
            return BadRequest("账号或密码错误!");

        }
        /// <summary>
        /// 登录后获取用户基础信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUserInfo()
        {
            var result = await dbContext.Users.Where(x => x.Id == userManager.GetUserId(User)).FirstOrDefaultAsync();
            if (result == null)
                return BadRequest("用户不存在!");
            //var wallet = await dbContext.UserWallets.Where(x => x.UserId == result.Id).FirstOrDefaultAsync();
            var claims = await userManager.GetClaimsAsync(result);
            return Ok(new
            {
                result.Id,
                result.NickName,
                //result.Picture,
                result.PhoneNumber,
                result.BirthDate,
                result.Gender,
                Claims = claims.ToDictionary(x => x.Type, x => x.Value),
                //Balance = wallet?.Balance ?? 0
            });
        }
        /// <summary>
        /// 手机号登录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SignWithPhoneNumber([FromBody] SignInWithPhonenumberModel model)
        {
            var user = await userManager.FindByNameAsync(model.Phonenumber);
            if (user == null)
            {
                user = new User();
                user.Id = string.Empty;
                await userManager.SetPhoneNumberAsync(user, model.Phonenumber);
                await userManager.SetUserNameAsync(user, model.Phonenumber);
                user.SecurityStamp = EmptyUserSecurityStamp;
            }
            //var authenticateResult = await HttpContext.AuthenticateAsync();
            //var externalLoginInfo = await signInManager.GetExternalLoginInfoAsync();
            //if (await userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultPhoneProvider, $"SignIn_{model.PhoneNumber}", model.Code))
            //{
            //如果数据库里账号不存在创建账号
            if (string.IsNullOrWhiteSpace(await userManager.GetUserIdAsync(user)))
            {
                user.Id = Guid.NewGuid().ToString();
                user.NickName = model?.NickName;
                //user.Picture = model?.Picture;
                //user.Identity = National.Enum.IdentityEnum.None;
                await userManager.CreateAsync(user);
                await userManager.SetLockoutEnabledAsync(user, false);
                await userManager.AddToRoleAsync(user, "前端用户");
                if (!string.IsNullOrWhiteSpace(model.OpenId))
                {
                    await userManager.AddClaimAsync(user, new Claim("wechat_openId", model.OpenId));
                }
                //dbContext.Add(new UserWallet
                //{
                //    UserId = user.Id,
                //    Balance = 0
                //});
                await dbContext.SaveChangesAsync();

            }
            if (await userManager.GetLockoutEnabledAsync(user))
                throw new Exception("账号已锁定!");
            if (await signInManager.CanSignInAsync(user))
            {
                //var claims = await signInManager.ClaimsFactory.CreateAsync(user);
                //var userPrincipal = await signInManager.CreateUserPrincipalAsync(user);
                //await signInManager.SignInAsync(user, isPersistent: true);
                var Roles = await userManager.GetRolesAsync(user);
                return Ok(GenerateJwt(new Claim[] {
                    new Claim(JwtClaimTypes.Subject,user.Id),
                    new Claim(JwtClaimTypes.NickName,user?.NickName ?? string.Empty),
                    new Claim(JwtClaimTypes.Role,Roles.Any()?string.Join(",",Roles):"")
                }));
            }
            //}
            return BadRequest();
        }
        private string GenerateJwt(Claim[] claims, double? DefaultExpiresDays = 7)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(configuration["JwtBearerOptions:SecretKey"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(DefaultExpiresDays.GetValueOrDefault()),
                Issuer = jwtOptions.TokenValidationParameters.ValidIssuer,
                Audience = jwtOptions.TokenValidationParameters.ValidAudience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),

            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
