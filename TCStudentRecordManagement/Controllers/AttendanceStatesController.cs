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

[Route("[Controller]")]
[Authorize]
[ApiController]
public class AttendanceStatesController : ControllerBase
{
    private readonly DataContext _context;

    public AttendanceStatesController(DataContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get a list of all the records from the AttendanceState table
    /// </summary>
    /// <returns></returns>
    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<AttendanceStateDTO>>> List()
    {

        // Convert AttendanceState to AttendanceStateDTO
        List<AttendanceState> attendanceStateData = await _context.AttendanceStates.ToListAsync();
        List<AttendanceStateDTO> result = new List<AttendanceStateDTO>();
        attendanceStateData.ForEach(x => result.Add(new AttendanceStateDTO(x)));

        // Log to debug log
        Logger.Msg<AttendanceStatesController>($"[{User.FindFirstValue("email")}] [LIST]", Serilog.Events.LogEventLevel.Debug);
        return result;

    } // End of List

    /// <summary>
    /// Add an AttendanceState record
    /// </summary>
    /// <param name="description"></param>
    /// <returns></returns>
    [HttpPut("add")]
    [Authorize(Policy = "StaffMember")]
    public async Task<ActionResult> AddAttendanceState(string description)
    {

        // Call BLL TaskType Add method with all the parameters
        object BLLResponse = new AttendanceStateBLL(_context).AddAttendanceStateBLL(description: description);

        // Get the base class for the response
        // Ref: https://docs.microsoft.com/en-us/dotnet/api/system.type.basetype?view=netcore-3.1
        if (BLLResponse.GetType().BaseType == typeof(Exception))
        {
            // Create log entries for Debug log
            ((APIException)BLLResponse).Exceptions.ForEach(ex => Logger.Msg<AttendanceStatesController>((Exception)ex, Serilog.Events.LogEventLevel.Debug));

            // Return response from API
            return BadRequest(new { errors = ((APIException)BLLResponse).Exceptions.Select(x => x.Message).ToArray() });
        }
        else
        {
            try
            {
                AttendanceState newAttendanceState = new AttendanceState { Description = ((AttendanceStateDTO)BLLResponse).Description };

                // Create the record
                _context.AttendanceStates.Add(newAttendanceState);
                await _context.SaveChangesAsync();

                Logger.Msg<AttendanceStatesController>($"[{User.FindFirstValue("email")}] [ADD] TaskType '{description}' successful", Serilog.Events.LogEventLevel.Information);

                // Convert back to DTO and return to user
                AttendanceStateDTO response = new AttendanceStateDTO(newAttendanceState);
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Local log entry. Database reconciliation issues are more serious so reported as Error
                Logger.Msg<AttendanceStatesController>($"[ADD] Database sync error {ex.Message}", Serilog.Events.LogEventLevel.Error);

                // Return response to client
                return StatusCode(500, new { errors = "Database update failed. Contact the administrator to resolve this issue." });
            }
        }

    } // End of AddAttendanceState

    /// <summary>
    /// Modify an existing AttendanceState record
    /// </summary>
    /// <param name="attendanceState"></param>
    /// <returns></returns>
    [HttpPut("modify")]
    [Authorize(Policy = "StaffMember")]
    public async Task<ActionResult> ModifyAttendanceState([FromBody] AttendanceStateDTO attendanceState)
    {
        if (AttendanceStateExists(attendanceState.StateID))
        {

            // Call BLL TaskType Modify method with all the parameters
            object BLLResponse = new AttendanceStateBLL(_context).ModifyAttendanceStateBLL(attendanceState: attendanceState);

            if (BLLResponse.GetType().BaseType == typeof(Exception))
            {
                // Create log entries for Debug log
                ((APIException)BLLResponse).Exceptions.ForEach(ex => Logger.Msg<AttendanceStatesController>((Exception)ex, Serilog.Events.LogEventLevel.Debug));

                // Return response from API
                return BadRequest(new { errors = ((APIException)BLLResponse).Exceptions.Select(x => x.Message).ToArray() });
            }
            else
            {
                try
                {
                    AttendanceStateDTO modAttendanceState = (AttendanceStateDTO)BLLResponse;

                    // Find the existing record based on ID
                    AttendanceState currentRecord = _context.AttendanceStates.Where(x => x.StateID == modAttendanceState.StateID).First();

                    // Modify the record
                    currentRecord.Description = modAttendanceState.Description;

                    // Save changes
                    await _context.SaveChangesAsync();

                    Logger.Msg<AttendanceStatesController>($"[{User.FindFirstValue("email")}] [MODIFY] StateID: {currentRecord.StateID} successful", Serilog.Events.LogEventLevel.Information);

                    // Return modified record as a DTO
                    AttendanceStateDTO response = new AttendanceStateDTO(currentRecord);
                    return Ok(response);
                }
                catch (Exception ex)
                {
                    // Local log entry. Database reconciliation issues are more serious so reported as Error
                    Logger.Msg<AttendanceStatesController>($"[MODIFY] Database sync error {ex.Message}", Serilog.Events.LogEventLevel.Error);

                    // Return response to client
                    return StatusCode(500, new { errors = "Database update failed. Contact the administrator to resolve this issue." });
                }
            }
        }
        else
        {
            return NotFound();
        }
    } // End of ModifyAttendanceState

    /// <summary>
    /// Deletes the AttendanceState record, provided that there are no FK dependencies
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("delete")]
    [Authorize(Policy = "SuperAdmin")]
    public async Task<ActionResult> Delete(int id)
    {
        // Find existing TaskType record in DB
        AttendanceState attendanceState = await _context.AttendanceStates.FindAsync(id);

        if (attendanceState == null)
        {
            Logger.Msg<AttendanceStatesController>($"[{User.FindFirstValue("email")}] [DELETE] StateID: {id} not found", Serilog.Events.LogEventLevel.Debug);
            return NotFound();
        }

        try
        {
            _context.AttendanceStates.Remove(attendanceState);
            await _context.SaveChangesAsync();

            Logger.Msg<AttendanceStatesController>($"[{User.FindFirstValue("email")}] [DELETE] StateID: {id} success", Serilog.Events.LogEventLevel.Information);
            return Ok(new AttendanceStateDTO(attendanceState));
        }
        catch (Exception ex)
        {
            // Probably due to FK violation
            Logger.Msg<AttendanceStatesController>($"[DELETE] Database sync error {ex.Message}", Serilog.Events.LogEventLevel.Error);

            // Return response to client
            return StatusCode(500, new { errors = "Database update failed." });
        }

    }

    private bool AttendanceStateExists(int id)
    {
        return _context.AttendanceStates.Any(e => e.StateID == id);
    }
}