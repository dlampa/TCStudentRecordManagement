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
    [Authorize]
    [ApiController]
    public class TimesheetsController : ControllerBase
    {
        private readonly DataContext _context;

        public TimesheetsController(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Add a new Timesheet record to the Timesheets table
        /// </summary>
        /// <param name="timesheet"></param>
        /// <returns></returns>
        [Route("add")]
        [Authorize(Policy = "StaffMember")]
        public async Task<ActionResult> AddTimesheetRecord(TimesheetDTO timesheetRecord)
        {

            // Call BLL Task Add method with all the parameters
            object BLLResponse = new TimesheetBLL(_context).AddTimesheetRecordBLL(timesheetRecord: timesheetRecord);

            if (BLLResponse.GetType().BaseType == typeof(Exception))
            {
                // Create log entries for Debug log
                ((APIException)BLLResponse).Exceptions.ForEach(ex => Logger.Msg<TimesheetsController>((Exception)ex, Serilog.Events.LogEventLevel.Debug));

                // Return response from API
                return BadRequest(new { errors = ((APIException)BLLResponse).Exceptions.Select(x => x.Message).ToArray() });
            }
            else
            {
                try
                {
                    TimesheetDTO newTimesheetDTO = (TimesheetDTO)BLLResponse;

                    Timesheet newTimesheet = new Timesheet
                    {
                        StudentID = newTimesheetDTO.StudentID,
                        AssignmentID = newTimesheetDTO.AssignmentID,
                        Date = newTimesheetDTO.Date,
                        TimeAllocation = newTimesheetDTO.TimeAllocation
                    };

                    _context.Timesheets.Add(newTimesheet);
                    await _context.SaveChangesAsync();

                    Logger.Msg<TimesheetsController>($"[{User.FindFirstValue("email")}] [ADD] Timesheet '{newTimesheet.RecordID}' successful", Serilog.Events.LogEventLevel.Information);

                    // Convert back to DTO and return to user
                    TimesheetDTO response = new TimesheetDTO(newTimesheet);
                    return Ok(response);
                }
                catch (Exception ex)
                {
                    // Local log entry. Database reconciliation issues are more serious so reported as Error
                    Logger.Msg<TimesheetsController>($"[ADD] Database sync error {ex.Message}", Serilog.Events.LogEventLevel.Error);

                    // Return response to client
                    return StatusCode(500, new { errors = "Database update failed. Contact the administrator to resolve this issue." });
                }
            }

        } // End of AddTimesheetRecord

        /// <summary>
        /// Modify an existing Timesheet record
        /// </summary>
        /// <param name="timesheetRecord"></param>
        /// <returns></returns>
        [HttpPut("modify")]
        [Authorize(Policy = "StaffMember")]
        public async Task<ActionResult> ModifyTask([FromBody] TimesheetDTO timesheet)
        {
            if (TimesheetRecordExists(timesheet.RecordID))
            {
                // Call BLL TimesheetRecord Modify method with all the parameters
                object BLLResponse = new TimesheetBLL(_context).ModifyTimesheetRecordBLL(timesheetModRecord: timesheet);

                if (BLLResponse.GetType().BaseType == typeof(Exception))
                {
                    // Create log entries for Debug log
                    ((APIException)BLLResponse).Exceptions.ForEach(ex => Logger.Msg<TimesheetsController>((Exception)ex, Serilog.Events.LogEventLevel.Debug));

                    // Return response from API
                    return BadRequest(new { errors = ((APIException)BLLResponse).Exceptions.Select(x => x.Message).ToArray() });
                }
                else
                {
                    try
                    {
                        TimesheetDTO modTimesheet = (TimesheetDTO)BLLResponse;

                        // Find the existing record based on ID
                        Timesheet currentRecord = _context.Timesheets.Where(x => x.RecordID == modTimesheet.RecordID).First();

                        // Modify the record
                        currentRecord.AssignmentID = modTimesheet.AssignmentID;
                        currentRecord.TimeAllocation = modTimesheet.TimeAllocation;

                        // Save changes
                        await _context.SaveChangesAsync();

                        Logger.Msg<TimesheetsController>($"[{User.FindFirstValue("email")}] [MODIFY] RecordID: {currentRecord.RecordID} successful", Serilog.Events.LogEventLevel.Information);

                        // Return modified record as a DTO
                        TimesheetDTO response = new TimesheetDTO(currentRecord);
                        return Ok(response);
                    }
                    catch (Exception ex)
                    {
                        // Local log entry. Database reconciliation issues are more serious so reported as Error
                        Logger.Msg<TimesheetsController>($"[MODIFY] Database sync error {ex.Message}", Serilog.Events.LogEventLevel.Error);

                        // Return response to client
                        return StatusCode(500, new { errors = "Database update failed. Contact the administrator to resolve this issue." });
                    }
                }
            }
            else
            {
                return NotFound();
            }
        } // End of ModifyTimesheetRecord


        private bool TimesheetRecordExists(int id)
        {
            return _context.Timesheets.Any(e => e.RecordID == id);
        }
    }
}
