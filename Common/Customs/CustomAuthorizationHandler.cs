using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Common;

public class CustomAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IHttpContextAccessor _accessor;

    public CustomAuthorizationHandler(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }
    private static string getSubType="http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        //查詢用戶角色数据,判断角色可以访问哪些api
        var token = _accessor.HttpContext.Request.Headers["Authorization"].ToString();
        var userId= _accessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == getSubType)?.Value;
        //if (requirement.Permissions.Any(x=>x.Role==RoleHelper.FreeRepRole))
        if(userId== "C0EEEE5B1B334BDD83D9B6AE2C27DCDE")
        {
             context.Succeed(requirement);
             
        }
        else
            context.Fail();
    }
}