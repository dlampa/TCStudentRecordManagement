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

        [HttpGet("get")]
        public async Task<ActionResult<IEnumerable<Models.Task>>> Get(int recordID, int studentID, int unitID, int typeID, int assignmentID, int cohortID, DateTime startDate, DateTime endDate)
        {
            object BLLResponse = new TimesheetBLL(_context).GetTimesheetRecordBLL(recordID: recordID, studentID: studentID, unitID: unitID, typeID: typeID, assignmentID: assignmentID, cohortID: cohortID, startDate: startDate, endDate: endDate, userClaims: User);

            if (BLLResponse.GetType().BaseType == typeof(Exception))
            {
                // Create log entries for Debug log
                ((APIException)BLLResponse).Exceptions.ForEach(ex => Logger.Msg<AttendancesController>((Exception)ex, Serilog.Events.LogEventLevel.Debug));

                // Return response from API
                return BadRequest(new { errors = ((APIException)BLLResponse).Exceptions.Select(x => x.Message).ToArray() });
            }
            else
            {
                try
                {

                    TimesheetGetDTO BLLResponseDTO = (TimesheetGetDTO)(BLLResponse);

                    // Apply all the criteria with supplied or default values from BLL
                    IQueryable<Timesheet> dbRequest = _context.Timesheets
                        .Where(x => x.Date >= BLLResponseDTO.StartDate && x.Date <= BLLResponseDTO.EndDate);

                    if (BLLResponseDTO.StudentID != 0) dbRequest = dbRequest.Where(x => x.StudentID == BLLResponseDTO.StudentID);
                    if (BLLResponseDTO.CohortID != 0) dbRequest = dbRequest.Include(timesheet => timesheet.ForStudent).Where(x => x.ForStudent.CohortID == BLLResponseDTO.CohortID);
                    if (BLLResponseDTO.TypeID != 0) dbRequest = dbRequest.Include(timesheet => timesheet.AssignmentAlloc).Where(x => x.AssignmentAlloc.TypeID == BLLResponseDTO.TypeID);
                    if (BLLResponseDTO.UnitID != 0) dbRequest = dbRequest.Include(timesheet => timesheet.AssignmentAlloc).Where(x => x.AssignmentAlloc.UnitID == BLLResponseDTO.UnitID);
                    if (BLLResponseDTO.AssignmentID != 0) dbRequest = dbRequest.Include(timesheet => timesheet.AssignmentAlloc).Where(x => x.AssignmentAlloc.TaskID == BLLResponseDTO.AssignmentID);

                    List<Timesheet> dbResponse = await dbRequest.ToListAsync();

                    // Convert result to TimesheetGetDTO
                    List<TimesheetGetDTO> response = new List<TimesheetGetDTO>();
                    dbResponse.ForEach(x => response.Add(new TimesheetGetDTO(x)));

                    Logger.Msg<TimesheetsController>($"[{User.FindFirstValue("email")}] [GET] ", Serilog.Events.LogEventLevel.Debug);
                    return Ok(response);
                }
                catch (Exception ex)
                {
                    // Local log entry. Database reconciliation issues are more serious so reported as Error
                    Logger.Msg<TimesheetsController>($"[GET] Database sync error {ex.Message}", Serilog.Events.LogEventLevel.Error);

                    // Return response to client
                    return StatusCode(500, new { errors = "Database update failed. Contact the administrator to resolve this issue." });
                }
            }
        }// End of Get
        

        /// <summary>
        /// Add a new Timesheet record to the Timesheets table
        /// </summary>
        /// <param name="timesheet"></param>
        /// <returns></returns>
        [HttpPut("add")]
        [Authorize(Policy = "StaffMember")]
        public async Task<ActionResult> AddTimesheetRecord(TimesheetDTO timesheetRecord)
        {

            // Call BLL Task Add method with all the parameters
            object BLLResponse = new TimesheetBLL(_context).AddTimesheetRecordBLL(timesheetRecord: timesheetRecord, userClaims: User);

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
        public async Task<ActionResult> ModifyTimesheetRecord([FromBody] TimesheetDTO timesheet)
        {
            if (TimesheetRecordExists(timesheet.RecordID))
            {
                // Call BLL TimesheetRecord Modify method with all the parameters
                object BLLResponse = new TimesheetBLL(_context).ModifyTimesheetRecordBLL(timesheetModRecord: timesheet, userClaims: User);

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

        /// <summary>
        /// Deletes the Timesheet record
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("delete")]
        public async Task<ActionResult> DeleteTimesheetRecord(int id)
        {
            // Find existing Timesheet record in DB
            Timesheet timesheet = await _context.Timesheets.FindAsync(id);

            if (timesheet != null)
            {
                // Call BLL TimesheetRecord Modify method with all the parameters
                object BLLResponse = new TimesheetBLL(_context).DeleteTimesheetRecordBLL(recordID: id, userClaims: User);

                if (BLLResponse.GetType().BaseType == typeof(Exception))
                {
                    // Create log entries for Debug log
                    ((APIException)BLLResponse).Exceptions.ForEach(ex => Logger.Msg<TimesheetsController>((Exception)ex, Serilog.Events.LogEventLevel.Information));

                    // Return response from API
                    return BadRequest(new { errors = ((APIException)BLLResponse).Exceptions.Select(x => x.Message).ToArray() });
                }
                else
                {
                    try
                    {
                        _context.Timesheets.Remove(timesheet);
                        await _context.SaveChangesAsync();

                        Logger.Msg<TimesheetsController>($"[{User.FindFirstValue("email")}] [DELETE] RecordID: {id} success", Serilog.Events.LogEventLevel.Information);
                        return Ok(new TimesheetDTO(timesheet));
                    }
                    catch (Exception ex)
                    {
                        Logger.Msg<TimesheetsController>($"[DELETE] Database sync error {ex.Message}", Serilog.Events.LogEventLevel.Error);

                        // Return response to client
                        return StatusCode(500, new { errors = "Database update failed." });
                    }
                }
            }
            else
            {
                Logger.Msg<TimesheetsController>($"[{User.FindFirstValue("email")}] [DELETE] RecordID: {id} not found", Serilog.Events.LogEventLevel.Debug);
                return NotFound();
            }

        } // End of Delete



        /// <summary>
        /// Checks for existence of Timesheet records in the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool TimesheetRecordExists(int id)
        {
            return _context.Timesheets.Any(e => e.RecordID == id);
        }
    }
}
