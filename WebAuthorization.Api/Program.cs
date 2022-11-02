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
    c.SwaggerDoc("api", new OpenApiInfo { Title = "API�ĵ�" });
    string[] files = Directory.GetFiles(AppContext.BaseDirectory, "*.xml");
    foreach (var item in files)
    {
        c.IncludeXmlComments(item, true);
    }
    //����ȨС��
    c.OperationFilter<AddResponseHeadersFilter>();
    c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
    // ��header�����token�����ݵ���̨
    c.OperationFilter<SecurityRequirementsOperationFilter>();
    // Jwt Bearer ��֤�������� oauth2
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "������Ȩ(���ݽ�������ͷ�н��д���) ֱ�����¿�������Bearer {token}��ע������֮����һ���ո�\"",
        Name = "Authorization",//jwtĬ�ϵĲ�������
        In = ParameterLocation.Header,//jwtĬ�ϴ��Authorization��Ϣ��λ��(����ͷ��)
        Type = SecuritySchemeType.ApiKey
    });
});
var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes("c2cf27850b184180a7f6611551ebda20")), SecurityAlgorithms.HmacSha256);

// ���Ҫ���ݿ⶯̬�󶨣������������գ���ߴ������ﶯ̬��ֵ
var permission = new List<PermissionItem>();

// ��ɫ��ӿڵ�Ȩ��Ҫ�����
var permissionRequirement = new PermissionRequirement(
    "/api/denied",
    permission,
    ClaimTypes.Role,
    "wingwell",//������
    "Jiesen_Audience",//����
    signingCredentials,//ǩ��ƾ��
    expiration: TimeSpan.FromSeconds(60 * 60)//�ӿڵĹ���ʱ��
);
builder.Services.AddAuthorization(options =>
{
    //��Ȩ��������� Policy�����ԣ���Roles����ɫ�� �� AuthenticationSchemes��������,�Ƽ�Policy

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
            ValidateIssuer = true, //�Ƿ���֤Issuer
            ValidIssuer = configuration["Jwt:Issuer"], //������Issuer
            ValidateAudience = true, //�Ƿ���֤Audience
            ValidAudience = configuration["Jwt:Audience"], //������Audience
            ValidateIssuerSigningKey = true, //�Ƿ���֤SecurityKey
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"])), //SecurityKey
            ValidateLifetime = true, //�Ƿ���֤ʧЧʱ��
            ClockSkew = TimeSpan.FromSeconds(30), //����ʱ���ݴ�ֵ�������������ʱ�䲻ͬ�����⣨�룩
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
        c.SwaggerEndpoint("/swagger/api/swagger.json", "api �ĵ�");
    });
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
