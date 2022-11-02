using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Common;

public class RolesAuthorizationHandler : AuthorizationHandler<RolesAuthorizationRequirement>, IAuthorizationRequirement
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, RolesAuthorizationRequirement requirement)
    {

        if (requirement.AllowedRoles.Any(r => context.User.IsInRole(r)))
        {
            context.Succeed(requirement);
        }
        else
            context.Fail();
    }
}