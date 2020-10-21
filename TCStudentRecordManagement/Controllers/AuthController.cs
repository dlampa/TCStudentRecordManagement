using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TCStudentRecordManagement.Models;
using TCStudentRecordManagement.Utils;

namespace TCStudentRecordManagement.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly DataContext _context;

        public AuthController(DataContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Process user logon information from the frontend and return information about the user from the database
        /// that's used for enhancing user experience
        /// </summary>
        /// <returns></returns>
        [HttpGet("logon")]
        public async Task<ActionResult> Logon() 
        {
            if (User.Identity.IsAuthenticated)
            {
                // Check if the user is a member of any of the elevated rights groups
                int localcohortID = 0;

                // If the user is a student, get their cohortID
                if (!(User.IsInRole("Staff") || User.IsInRole("SuperAdmin"))) {
                    localcohortID = _context.Users.Where(x => x.Email == User.FindFirstValue("email")).Include(user => user.StudentData).FirstOrDefault()?.StudentData.CohortID ?? 0;
                }

                // Create a response object

                object authResponse = new
                {
                    email = User.FindFirstValue("email"),
                    fullname = User.Identity.Name,
                    groupMembership = User.FindFirstValue(ClaimTypes.Role),
                    tokenID = _context.Users.Where(x => x.Email == User.FindFirstValue("email")).First().ActiveToken,
                    imageURL = User.FindFirstValue("website"),
                    cohortID = localcohortID
                };

                Logger.Msg<AuthController>($"[LOGIN] SUCCESS User {User.FindFirstValue("email")} ({User.FindFirstValue(ClaimTypes.Role)})");
                return Ok(authResponse);

            }
            else
            {
                Logger.Msg<AuthController>($"[LOGIN] SUCCESS User {User.FindFirstValue("email")} ({User.FindFirstValue(ClaimTypes.Role)})", Serilog.Events.LogEventLevel.Warning);
                return Forbid();
            }
        } // End of Logon

    }
}
