using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TCStudentRecordManagement.Models;
using System.Security.Cryptography.X509Certificates;

namespace TCStudentRecordManagement.Auth.Authorization
{
    public class StaffAuthCheck : IAuthorizationRequirement
    {
        public StaffAuthCheck()
        {

        }
    }

    public class StaffAuthCheckHandler : AuthorizationHandler<StaffAuthCheck>
    {
        // Will need this to pass HTTP Request information to the AuthorizationHandler
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StaffAuthCheckHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // Use HandleRequirementAsync to decide if the authorization is granted or not
        protected async override System.Threading.Tasks.Task HandleRequirementAsync(AuthorizationHandlerContext context, StaffAuthCheck requirement)
        {
            // Note: the method will run synchronously because there is nothing async in it's operation

            // Fail immediately if the user is not authenticated.
            if (!context.User.Identity.IsAuthenticated)
            {
                context.Fail();
                return;
            }

            // Check if the user is a Staff group member or a Super Admin (Super Admin supercedes Staff rights, so it's acceptable)
            if (context.User.Claims.Where(x => x.Type == "Role").Select(x => x.Value == "Staff" | x.Value == "Super").First())
            {
                // Return success
                context.Succeed(requirement);
                return;
            }
            else
            {
                // Return fail
                context.Fail();
                return;
            }

        }

    }

}
