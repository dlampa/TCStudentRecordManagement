using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TCStudentRecordManagement.Models;
using TCStudentRecordManagement.Models.DTO;
using TCStudentRecordManagement.Utils;

namespace TCStudentRecordManagement.Controllers
{
    [Route("[controller]")]
    [Authorize(Policy = "SuperAdmin")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly DataContext _context;

        public StaffController(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all Staff records from the database and reports them as StaffDTO object
        /// </summary>
        /// <returns>JSON Array of StaffDTO objects</returns>
        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<StaffDTO>>> List()
        {
            // Convert Staff to StaffDTO
            List<Staff> staffData = await _context.Staff.ToListAsync();
            List<StaffDTO> result = new List<StaffDTO>();
            staffData.ForEach(x => result.Add(new StaffDTO(x)));

            // Log to debug log
            Logger.Msg<StaffController>($"[{User.FindFirstValue("email")}] [LIST]", Serilog.Events.LogEventLevel.Debug);
            if (result == null)
            {
                return NotFound();
            }
            else
            {
                return result;
            }

        } // End of List

        /// <summary>
        /// Retrieves staff records from the database based on StaffID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("get")]
        public async Task<ActionResult<UserDTO>> Get(int id)
        {
            // Convert User to UserDTO
            User userData = await _context.Users.FindAsync(id);
            UserDTO result = new UserDTO(userData);

            // Log to debug log
            Logger.Msg<UsersController>($"[{User.FindFirstValue("email")}] [GET]", Serilog.Events.LogEventLevel.Debug);

            if (result == null)
            {
                return NotFound();
            }
            else
            {
                return result;
            }

        } // End of Get

       

        private bool StaffExists(int id)
        {
            return _context.Staff.Any(e => e.StaffID == id);
        }
    }
}
