using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using WebAuthorization.Api;

var builder = WebApplication.CreateBuilder(args);
var apiName = "test";
var apiEmail = "test@email.com";
var apiUrl = "www.baidu.com";
// Add services to the container.
var configuration = builder.Configuration;
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("api", new OpenApiInfo { Title = "API文档" });
    string[] files = Directory.GetFiles(AppContext.BaseDirectory, "*.xml");
    foreach (var item in files)
    {
        c.IncludeXmlComments(item, true);
    }
    //启加权小锁
    c.OperationFilter<AddResponseHeadersFilter>();
    c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
    // 在header中添加token，传递到后台
    c.OperationFilter<SecurityRequirementsOperationFilter>();
    // Jwt Bearer 认证，必须是 oauth2
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "请求授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）\"",
        Name = "Authorization",//jwt默认的参数名称
        In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
        Type = SecuritySchemeType.ApiKey
    });
});
var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes("c2cf27850b184180a7f6611551ebda20")), SecurityAlgorithms.HmacSha256);

// 如果要数据库动态绑定，这里先留个空，后边处理器里动态赋值
var permission = new List<PermissionItem>();

// 角色与接口的权限要求参数
var permissionRequirement = new PermissionRequirement(
    "/api/denied",
    permission,
    ClaimTypes.Role,
    "wingwell",//发行人
    "Jiesen_Audience",//听众
    signingCredentials,//签名凭据
    expiration: TimeSpan.FromSeconds(60 * 60)//接口的过期时间
);
builder.Services.AddAuthorization(options =>
{
    //授权规则可以是 Policy（策略）、Roles（角色） 或 AuthenticationSchemes（方案）,推荐Policy

    //options.AddPolicy(Permissions.FreerepName, policy => policy.RequireClaim("UnionId"));
    //options.AddPolicy(Permissions.FreerepName, policy => policy.Requirements.Add(new RolesAuthorizationHandler()));
    //options.AddPolicy(Permissions.WingWell,
    //    policy => policy.Requirements.Add(new RolesAuthorizationRequirement(new[] { RoleHelper.FreeRepRole, RoleHelper.ContentRole })));

    options.AddPolicy(Permissions.WingWell, policy => policy.Requirements.Add(permissionRequirement));
    options.AddPolicy(Permissions.ContentName, policy => policy.RequireClaim("RoleId", "5"));
});

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = nameof(ApiResponseHandler);
        options.DefaultForbidScheme = nameof(ApiResponseHandler);
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true, //是否验证Issuer
            ValidIssuer = configuration["Jwt:Issuer"], //发行人Issuer
            ValidateAudience = true, //是否验证Audience
            ValidAudience = configuration["Jwt:Audience"], //订阅人Audience
            ValidateIssuerSigningKey = true, //是否验证SecurityKey
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"])), //SecurityKey
            ValidateLifetime = true, //是否验证失效时间
            ClockSkew = TimeSpan.FromSeconds(30), //过期时间容错值，解决服务器端时间不同步问题（秒）
            RequireExpirationTime = true,
        };
    }).AddScheme<AuthenticationSchemeOptions, ApiResponseHandler>(nameof(ApiResponseHandler), o => { }); ;
builder.Services.AddSingleton(new JwtHelper(configuration));
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
//builder.Services.AddScoped<IAuthorizationHandler, RolesAuthorizationHandler>();
builder.Services.AddScoped<IAuthorizationHandler, CustomAuthorizationHandler>();
var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/api/swagger.json", "api 文档");
    });
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
