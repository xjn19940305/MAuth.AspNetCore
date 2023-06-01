using MAuth.AspNetCore.Database.Entities;
using MAuth.AspNetCore.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MAuth.AspNetCore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager
            )
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
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
    }
}
