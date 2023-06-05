using MAuth.AspNetCore.Database.Entities;
using MAuth.AspNetCore.MySql;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Security.Claims;

namespace MAuth.AspNetCore.Api.Services
{
    public class MyUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<User, Database.Entities.Role>
    {

        public MyUserClaimsPrincipalFactory( UserManager<User> userManager, RoleManager<Database.Entities.Role> roleManager, IOptions<IdentityOptions> options) : base(userManager, roleManager, options)
        {

        }
        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            if (!string.IsNullOrWhiteSpace(user.Avatar))
            {
                identity.AddClaim(new Claim("avatar", user.Avatar));
            }
            if (!string.IsNullOrWhiteSpace(user.NickName))
            {
                identity.AddClaim(new Claim("nickname", user.NickName));
            }
            return identity;
        }

    }
}
