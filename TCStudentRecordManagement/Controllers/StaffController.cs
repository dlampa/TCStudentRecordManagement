using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TCStudentRecordManagement.Controllers.BLL;
using TCStudentRecordManagement.Controllers.Exceptions;
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

        [HttpPost]
        public async Task<ActionResult> AddStaff(int userID, bool superUser = false)
        {
            // Call BLL Student Add method with all the parameters
            object BLLResponse = new StaffBLL(_context).AddStaffBLL(userID: userID, superUser: superUser);

            if (BLLResponse.GetType().BaseType == typeof(Exception))
            {
                ((APIException)BLLResponse).Exceptions.ForEach(ex => Logger.Msg<StaffController>((Exception)ex, Serilog.Events.LogEventLevel.Debug));
                return BadRequest(new { errors = ((APIException)BLLResponse).Exceptions.Select(x => x.Message).ToArray() });
            }
            else
            {
                try
                {
                    Dictionary<string, object> BLLResponseDic = (Dictionary<string, object>)(BLLResponse);
                    int newStaffID;

                    Staff newStaff = new Staff
                    {
                        UserID = (int)BLLResponseDic["UserID"],
                        SuperUser = BLLResponseDic["SuperUser"].ToString() == "True"
                    };

                    _context.Staff.Add(newStaff);
                    await _context.SaveChangesAsync();
                    newStaffID = newStaff.StaffID;

                    Logger.Msg<StaffController>($"[{User.FindFirstValue("email")}] [ADD] staff '{BLLResponseDic["UserID"].ToString()}' successful", Serilog.Events.LogEventLevel.Information);
                    StaffDTO response = new StaffDTO(newStaff);
                    return Ok(response);

                }
                catch (Exception ex)
                {
                    Logger.Msg<StaffController>($"[ADD] Database sync error {ex.Message}", Serilog.Events.LogEventLevel.Error);
                    return StatusCode(500, new { errors = "Database update failed. Contact the administrator to resolve this issue." });
                }
            }

        } // End of AddStaff



        private bool StaffExists(int id)
        {
            return _context.Staff.Any(e => e.StaffID == id);
        }
    }
}
