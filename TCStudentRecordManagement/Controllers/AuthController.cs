using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TCStudentRecordManagement.Models;

namespace TCStudentRecordManagement.Controllers
{
    [Route("[controller]")]
    [Authorize]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DataContext _context;

        public AuthController(DataContext context)
        {
            _context = context;
        }


        [HttpGet("logon")]
        public async Task<ActionResult> Logon() 
        {
            if (User.Identity.IsAuthenticated)
            {
                // Check if the user is a member of any of the elevated rights groups
                int cohortID = 0;

                // If the user is a student, get their cohortID
                if (!(User.IsInRole("Staff") || User.IsInRole("SuperAdmin"))) {
                    cohortID = _context.Users.Where(x => x.Email == User.FindFirstValue("email")).Include(user => user.StudentData).FirstOrDefault()?.StudentData.CohortID ?? 0;
                }

                // Create a response object

                object authResponse = new
                {
                    email = User.FindFirstValue("email"),
                    fullname = User.FindFirstValue("name"),
                    groupMembership = User.FindFirstValue(ClaimTypes.Role),
                    cohortID = cohortID
                };

                return Ok(authResponse);

            }
            else
            {
                return Forbid();
            }
        }

    }
}
