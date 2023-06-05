# MAuth.AspNetCore
## 一套net7的RBAC权限框架 权限控制基于角色。控制菜单权限以及API访问权限。
### 因为这边只是一套基础RBAC的框架没有什么业务，只实现了用户、角色、模块以及模块授权的功能。数据访问使用了 EFCORE
### 同时前端展示了如何授权控制组件的显示隐藏、以及接口的访问权限。在测试这个页面，你可以通过登录不同的账号体验不同账号绑定角色的权限。例如milo的账号可以看到所有菜单也可以访问所有API，但是test001和test002都只能看到部分的菜单以及部分的API访问权限。test003用用test001+test002的权限。前端的权限设计可以到这里查看   [前端权限指引](https://fantastic-admin.gitee.io/guide/permission.html)
### 后台管理前端框架采用Fantastic-admin

### netcore后端通过注册授权策略来控制API的访问权限
```csharp
builder.Services.AddAuthorization(config =>
{
    //授权指定策略
    config.AddPolicy("Demo", policy =>
    {
        policy.RequireClaim("permission", "demo.demo1");
        policy.RequireClaim("permission", "demo.demo2");
    });
    config.AddPolicy("Demo1", policy => policy.RequireClaim("permission", "demo.demo1"));
    config.AddPolicy("Demo2", policy => policy.RequireClaim("permission", "demo.demo2"));
});
```
```csharp
        [Authorize(policy:"Demo1")]
        [HttpGet]
        public async Task<IActionResult> demo()
        {
            return Ok("这里拥有DEMO权限可以看见");
        }
        [Authorize(policy: "Demo2")]
        [HttpGet]
        public async Task<IActionResult> demo2()
        {
            return Ok("这里拥有DEMO2权限可以看见");
        }
        [Authorize(policy: "Demo")]
        [HttpGet]
        public async Task<IActionResult> demo3()
        {
            return Ok("这里拥有DEMO1和DEMO2权限可以看见");
        }
```

### 像这里 我注册了几个策略，然后再api action中加入注解去使用
### 前端的权限可以参考我上面的连接,我目前给的测试是这样配置的。
``` javascript

const routes: RouteRecordRaw = {
    path: '/test',
    component: Layout,
    redirect: '/test/test1',
    name: 'test',
    meta: {
        title: '测试',
        icon: 'ep:lock',
    },
    children: [
        {
            path: 'test1',
            name: 'test1',
            component: () => import('@/views/test/test.vue'),
            meta: {
                title: '测试页面1',
                sidebar: false,
                breadcrumb: false,
                activeMenu: '/tm',
                auth: ['demo.query', 'demo.demo1', 'demo.demo2']
            },
        },
    ],
}

```
### 这里的配置和api里的授权策略对应,因为我用的是identity身份框架。我将策略保存到roleClaims中,所以在授权策略中可以直接用claimValue的值去匹配。欢迎大家指点!


### 在配置中有个Init属性配置成true就可以初始化数据，会自动迁移生成数据库以及初始化数据。