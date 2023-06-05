using MAuth.AspNetCore.Database.Entities;
using MAuth.AspNetCore.MySql;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MAuth.AspNetCore.Api.Services
{
    public class InitJob
    {
        public async Task Init(IServiceScope scope)
        {
            var userManage = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
            var dbContext = scope.ServiceProvider.GetRequiredService<MAuthDbContext>();
            if (!await dbContext.Database.CanConnectAsync())
            {
                await dbContext.Database.MigrateAsync();
                await CreateRole(roleManager);
                await CreateUser(userManage);
            }

        }

        async Task CreateUser(UserManager<User> userManage)
        {
            var user = new User { UserName = "milo" };
            await userManage.AddPasswordAsync(user, "123456");
            await userManage.CreateAsync(user);
            await userManage.AddToRoleAsync(user, "超级管理员");

            user = new User { UserName = "test001" };
            await userManage.AddPasswordAsync(user, "123456");
            await userManage.CreateAsync(user);
            await userManage.AddToRoleAsync(user, "测试角色");

            user = new User { UserName = "test002" };
            await userManage.AddPasswordAsync(user, "123456");
            await userManage.CreateAsync(user);
            await userManage.AddToRoleAsync(user, "测试角色2");

            user = new User { UserName = "test003" };
            await userManage.AddPasswordAsync(user, "123456");
            await userManage.CreateAsync(user);
            await userManage.AddToRoleAsync(user, "测试角色3");
        }
        async Task CreateRole(RoleManager<Role> roleManager)
        {
            var role = new Role { Name = "超级管理员", Sort = 10 };
            await roleManager.CreateAsync(role);
            await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim("permission", "user.query"));
            await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim("permission", "user.create"));
            await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim("permission", "user.edit"));
            await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim("permission", "user.delete"));
            await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim("permission", "role.query"));
            await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim("permission", "role.create"));
            await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim("permission", "role.edit"));
            await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim("permission", "role.delete"));
            await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim("permission", "demo.query"));
            await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim("permission", "demo.demo1"));
            await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim("permission", "demo.demo2"));

            role = new Role { Name = "测试角色", Sort = 15 };
            await roleManager.CreateAsync(role);
            await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim("permission", "demo.query"));
            await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim("permission", "demo.demo1"));


            role = new Role { Name = "测试角色2", Sort = 20 };
            await roleManager.CreateAsync(role);
            await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim("permission", "demo.query"));
            await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim("permission", "demo.demo2"));

            role = new Role { Name = "测试角色3", Sort = 20 };
            await roleManager.CreateAsync(role);
            await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim("permission", "demo.query"));
            await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim("permission", "demo.demo1"));
            await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim("permission", "demo.demo2"));

        }
    }
}
