using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using TCStudentRecordManagement.Utils;


namespace TCStudentRecordManagement.Auth.Authorization
{
    public class SuperAuthCheck : IAuthorizationRequirement
    {
        public SuperAuthCheck()
        {

        }
    }

    public class SuperAuthCheckHandler : AuthorizationHandler<SuperAuthCheck>
    {
        // Will need this to pass HTTP Request information to the AuthorizationHandler
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SuperAuthCheckHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // Use HandleRequirementAsync to decide if the authorization is granted or not
        protected async override System.Threading.Tasks.Task HandleRequirementAsync(AuthorizationHandlerContext context, SuperAuthCheck requirement)
        {
            // Note: the method will run synchronously because there is nothing async in it's operation

            // Fail immediately if the user is not authenticated.
            if (!context.User.Identity.IsAuthenticated)
            {
                context.Fail();
                return;
            }

            // Check if the user is a a Super Admin
            if (context.User.Claims.Any(x => x.Type == System.Security.Claims.ClaimTypes.Role && x.Value == "SuperAdmin"))
            {
                // Return success
                context.Succeed(requirement);
                return;
            }
            else
            {
                // Return fail
                Logger.Msg<GoogleTokenValidator>($"[LOGIN] FAILURE User {context.User.FindFirst("email").Value} unsuccessful authorization for [SuperAdmin]");
                context.Fail();
                return;
            }

        }

    }

}
