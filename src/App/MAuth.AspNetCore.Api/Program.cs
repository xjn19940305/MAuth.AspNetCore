
using IGeekFan.AspNetCore.Knife4jUI;
using MAuth.AspNetCore.Api.Filters;
using MAuth.AspNetCore.Api.Services;
using MAuth.AspNetCore.Api.Swaggers;
using MAuth.AspNetCore.Database.Entities;
using MAuth.AspNetCore.Models.Options;
using MAuth.AspNetCore.Mongo;
using MAuth.AspNetCore.MySql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Minio;
using Newtonsoft.Json.Serialization;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .AddCommandLine(args)
    .AddEnvironmentVariables();

var configPath = $"{AppContext.BaseDirectory}Configs";
if (Directory.Exists(configPath))
{
    var files = Directory.EnumerateFiles(configPath, "*.json");
    foreach (var file in files)
    {
        builder.Configuration.AddJsonFile(file, true, true);
    }
}
// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
#region swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("management", new OpenApiInfo
    {
        Version = "v1",
        Title = "MAuth Management",
        Description = "Management Api",
        TermsOfService = new Uri("https://example.com/terms")
    });
    c.SwaggerDoc("user", new OpenApiInfo
    {
        Version = "v1",
        Title = "MAuth",
        Description = "User Api",
        TermsOfService = new Uri("https://example.com/terms")
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Description = "\u5728\u4e0b\u6846\u4e2d\u8f93\u5165\u8bf7\u6c42\u5934\u4e2d\u9700\u8981\u6dfb\u52a0\u004a\u0077\u0074\u6388\u6743\u0054\u006f\u006b\u0065\u006e\uff1a\u0042\u0065\u0061\u0072\u0065\u0072\u0020\u0054\u006f\u006b\u0065\u006e",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                    }
                });
    c.SchemaFilter<EnumSchemaFilter>();
    //c.CustomOperationIds(apiDesc =>
    //{
    //    var controllerAction = apiDesc.ActionDescriptor as ControllerActionDescriptor;
    //    return controllerAction.ActionName;
    //});

    typeof(ApiGroupNames).GetFields().Skip(1).ToList().ForEach(f =>
    {
        //获取枚举值上的特性
        var info = (GroupInfoAttribute)f.GetCustomAttributes(typeof(GroupInfoAttribute), false).FirstOrDefault();
        c.SwaggerDoc(f.Name, new OpenApiInfo
        {
            Title = info?.Title,
            Version = "v1",
            Description = info?.Title
        });
    });
    //判断接口归于哪个分组                    
    c.DocInclusionPredicate((docName, apiDescription) =>
    {
        //反射拿到值 
        var actionlist = apiDescription.ActionDescriptor.EndpointMetadata.Where(x => x is ApiGroupAttribute);
        if (actionlist.Count() > 0)
        {
            //判断是否包含这个分组 
            var actionfilter = actionlist.FirstOrDefault() as ApiGroupAttribute;
            if (actionfilter != null)
                return actionfilter.GroupName.Count(x => x.ToString().ToLower() == docName.ToLower()) > 0;
            else
                return false;
        }
        return false;
    });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath, true);
    var xmlMSLPModelFile = $"{Assembly.Load("MAuth.AspNetCore.Models").GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlMSLPModelFile), true);
});
#endregion

builder.Services.AddIdentity<User, Role>(options =>
{
    options.ClaimsIdentity.UserIdClaimType = "sub";
    options.ClaimsIdentity.RoleClaimType = "role";
    options.ClaimsIdentity.SecurityStampClaimType = "security_stamp";
    options.ClaimsIdentity.UserNameClaimType = "username";
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    //options.Lockout.MaxFailedAccessAttempts = 5;
})
 .AddDefaultTokenProviders()
 .AddClaimsPrincipalFactory<MyUserClaimsPrincipalFactory>()
 .AddEntityFrameworkStores<MAuthDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Events.OnRedirectToLogin = (ctx) =>
    {
        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = (ctx) =>
    {
        ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    };
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
});

builder.Services
    .AddAuthentication()
    .AddJwtBearer(options =>
    {
        if (options.SecurityTokenValidators.FirstOrDefault() is JwtSecurityTokenHandler jwtHandler)
        {
            jwtHandler.OutboundClaimTypeMap.Clear();
            jwtHandler.InboundClaimTypeMap.Clear();
        }
        builder.Configuration.Bind("JwtBearerOptions", options);
        //设置秘钥
        options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JwtBearerOptions:SecretKey"]));
    });
builder.Services.AddAuthorization(config =>
{
    //授权指定策略
    config.AddPolicy("Admin", policy =>
    {
        policy.RequireRole("Admin");
    });
    config.AddPolicy("Demo", policy =>
    {
        policy.RequireClaim("permission", "demo.demo1");
        policy.RequireClaim("permission", "demo.demo2");
    });

    config.AddPolicy("Demo1", policy => policy.RequireClaim("permission", "demo.demo1"));
    config.AddPolicy("Demo2", policy => policy.RequireClaim("permission", "demo.demo2"));

    config.AddPolicy("RoleQuery", policy => policy.RequireClaim("permission", "role.query"));
    config.AddPolicy("RoleCreate", policy => policy.RequireClaim("permission", "role.create"));
});
builder.Services
   .AddControllers(o => o.Filters.Add(typeof(ExceptionFilter)))
   .AddNewtonsoftJson(options =>
   {
       options.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
       options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
       options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
       options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
   });

#region 
builder.Services
   .AddHsts(o =>
   {
       o.IncludeSubDomains = true;
   })
   .AddCors(o =>
   {
       o.AddDefaultPolicy(p =>
       {
           p.SetIsOriginAllowed(domain => true);
           p.AllowAnyMethod();
           p.AllowAnyHeader();
           p.AllowCredentials();
       });
   })
   .Configure<ForwardedHeadersOptions>(options =>
   {
       options.ForwardedHeaders =
         ForwardedHeaders.All;
       options.KnownNetworks.Clear();
       options.KnownProxies.Clear();
   })
   .AddDataProtection().PersistKeysToStackExchangeRedis(
    StackExchange.Redis.ConnectionMultiplexer.Connect(builder.Configuration["DataProteciton:Redis:Connection"]),
    builder.Configuration["DataProteciton:Redis:Key"]);
#endregion 
#region mysql
var MySqlConnection = builder.Configuration["DB_CONNECTION"];
builder.Services.AddDbContext<MAuthDbContext>(setup =>
{
    setup.UseMySql(MySqlConnection, ServerVersion.AutoDetect(MySqlConnection), options =>
    {
        options.MigrationsAssembly(System.Reflection.Assembly.Load("MAuth.AspNetCore.MySql").FullName)
        .EnableRetryOnFailure(3, TimeSpan.FromSeconds(10), null);
    });
    //.LogTo(Console.WriteLine, LogLevel.Information);
});
#endregion 
#region redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["StackExchangeRedis:Connection"];
    options.InstanceName = builder.Configuration["StackExchangeRedis:Prefix"];
});
builder.Services.AddSingleton(sp =>
{
    return StackExchange.Redis.ConnectionMultiplexer.Connect(builder.Configuration["StackExchangeRedis:Connection"]);
});
#endregion


#region options
builder.Services.AddOptions<MongoOptions>()
  .BindConfiguration("MongoOptions");
builder.Services
    .AddOptions<FileSystemOptions>()
    .BindConfiguration("FileSystemOptions");
builder.Services.AddOptions<JwtBearerOptions>().BindConfiguration("JwtBearerOptions");
#endregion



#region 定时任务

#endregion

//依赖注入
builder.Services.AddScoped(s =>
{
    var options = s.GetRequiredService<IOptions<MongoOptions>>();
    return new MongoDB.Driver.MongoClient(options.Value.Connection);
});
builder.Services.AddScoped<MAuthMongoDbContext>();

builder.Services.AddSingleton(sp =>
{
    var options = sp.GetRequiredService<IOptions<FileSystemOptions>>().Value;
    return new MinioClient()
                       .WithEndpoint(options.Endpoint)
                       .WithCredentials(options.AccessKey, options.SecretKey)
                       //.WithSSL()
                       .Build();
});


var app = builder.Build();


if (builder.Configuration.GetValue<bool>("Init"))
{
    using var scope = app.Services.CreateScope();
    await new InitJob().Init(scope);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c => c.RouteTemplate = "swagger/{documentName}/swagger.json");
    //app.UseSwaggerUI(c =>
    //{
    //    c.SwaggerEndpoint("/swagger/management/swagger.json", "Manage");
    //    c.SwaggerEndpoint("/swagger/user/swagger.json", "User");
    //});
    app.UseKnife4UI(c =>
    {
        c.RoutePrefix = string.Empty;
        c.SwaggerEndpoint("/swagger/user/swagger.json", "用户端");
        c.SwaggerEndpoint("/swagger/management/swagger.json", "管理端");
    });
}
//app.UseStaticFiles();
//app.UseHttpsRedirection();
app.UseForwardedHeaders();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
