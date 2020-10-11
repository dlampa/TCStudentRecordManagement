using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using TCStudentRecordManagement.Utils;
using TCStudentRecordManagement.Models;
using System.Security.Claims;

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
            if (context.User.Claims.Where(x => x.Type == System.Security.Claims.ClaimTypes.Role && (x.Value == "Staff" | x.Value == "SuperAdmin")).Count() == 1)
            {
                // Return success
                context.Succeed(requirement);
                return;
            }
            else
            {
                // Return fail
                Logger.Msg<GoogleTokenValidator>($"[LOGIN] FAILURE User {context.User.FindFirst("email").Value} unsuccessful authorization for [Staff]");
                context.Fail();
                return;
            }

        }

    }

}
