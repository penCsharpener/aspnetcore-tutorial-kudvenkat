using kudvenkat.Utils;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kudvenkat.Security {
    public class SuperAdminHandler : AuthorizationHandler<ManageAdminRolesAndClaimsRequirement> {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       ManageAdminRolesAndClaimsRequirement requirement) {
            if (context.User.IsInRole(RoleConstants.SuperAdmin)) {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
