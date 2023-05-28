using MAuth.AspNetCore.Api.Filters;
using MAuth.AspNetCore.Api.Services;
using MAuth.AspNetCore.Database.Entities;
using MAuth.AspNetCore.Models.Options;
using MAuth.AspNetCore.Mongo;
using MAuth.AspNetCore.MySql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Minio;
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
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
    var xmlMSLPModelFile = $"{Assembly.Load("Marketing.Model").GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlMSLPModelFile));
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
        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
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
        options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("MAuth_2023_05_27"));
        // 配置Token验证参数
        //options.TokenValidationParameters = new TokenValidationParameters
        //{
        //    ValidateIssuer = true,
        //    ValidateAudience = true,
        //    ValidateLifetime = true,
        //    ValidateIssuerSigningKey = false,
        //    ValidIssuer = "marketing",
        //    ValidAudience = "marketing",
        //    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("marketing_2023_4_20"))
        //};
    });
builder.Services.AddAuthorization(config =>
{
    //授权指定策略
    config.AddPolicy("Admin", policy =>
    {
        policy.RequireRole("Admin");
    });
});
builder.Services
   .AddControllers(o => o.Filters.Add(typeof(ExceptionFilter)))
   .AddNewtonsoftJson(options =>
   {
       options.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
       options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
       options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
   });

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
using var scope = app.Services.CreateScope();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c => c.RouteTemplate = "swagger/{documentName}/swagger.json");
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/management/swagger.json", "Manage");
        c.SwaggerEndpoint("/swagger/user/swagger.json", "User");
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
