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
        /// <param name="id">Optional StaffID</param>
        /// <returns>JSON Array of StaffDTO objects</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StaffDTO>>> List(int id = -1)
        {
            List<StaffDTO> result = new List<StaffDTO>();

            if (id > -1)
            {
                Staff staffData = await _context.Staff.FindAsync(id);
                result.Add(new StaffDTO(staffData));

                Logger.Msg<StaffController>($"[${User.FindFirstValue("email")}] [GET] id={id}", Serilog.Events.LogEventLevel.Debug);
            }
            else
            {
                List<Staff> staffData = await _context.Staff.ToListAsync();
                staffData.ForEach(x => result.Add(new StaffDTO(x)));

                Logger.Msg<StaffController>($"[{User.FindFirstValue("email")}] [LIST]", Serilog.Events.LogEventLevel.Debug);
            }

            if (result == null)
            {
                return NotFound();
            }
            else
            {
                return result;
            }

        } // End of List

        private bool StaffExists(int id)
        {
            return _context.Staff.Any(e => e.StaffID == id);
        }
    }
}
