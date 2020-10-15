using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TCStudentRecordManagement.Controllers.BLL;
using TCStudentRecordManagement.Controllers.Exceptions;
using TCStudentRecordManagement.Models;
using TCStudentRecordManagement.Models.DTO;
using TCStudentRecordManagement.Utils;

namespace TCStudentRecordManagement.Controllers
{
    [Route("[Controller]")]
    [Authorize]
    [ApiController]
    public class UnitsController : ControllerBase
    {
        private readonly DataContext _context;

        public UnitsController(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get a list of all the records from the Units table
        /// </summary>
        /// <returns></returns>
        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<UnitDTO>>> List()
        {

            // Convert TaskType to TaskTypeDTO
            List<Unit> unitData = await _context.Units.ToListAsync();
            List<UnitDTO> result = new List<UnitDTO>();
            unitData.ForEach(x => result.Add(new UnitDTO(x)));

            // Log to debug log
            Logger.Msg<UnitsController>($"[{User.FindFirstValue("email")}] [LIST]", Serilog.Events.LogEventLevel.Debug);
            return result;

        }

        /// <summary>
        /// Add a learning Unit record
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        [HttpPut("add")]
        [Authorize(Policy = "StaffMember")]
        public async Task<ActionResult> AddUnit(string description)
        {

            // Call BLL Unit Add method with all the parameters
            object BLLResponse = new UnitBLL(_context).AddUnitBLL(description: description);

            if (BLLResponse.GetType().BaseType == typeof(Exception))
            {
                // Create log entries for Debug log
                ((APIException)BLLResponse).Exceptions.ForEach(ex => Logger.Msg<UnitsController>((Exception)ex, Serilog.Events.LogEventLevel.Debug));

                // Return response from API
                return BadRequest(new { errors = ((APIException)BLLResponse).Exceptions.Select(x => x.Message).ToArray() });
            }
            else
            {
                try
                {
                    Unit newUnit = new Unit { Description = ((UnitDTO)BLLResponse).Description };

                    // Create the record
                    _context.Units.Add(newUnit);
                    await _context.SaveChangesAsync();

                    Logger.Msg<UnitsController>($"[{User.FindFirstValue("email")}] [ADD] Unit '{description}' successful", Serilog.Events.LogEventLevel.Information);

                    // Convert back to DTO and return to user
                    UnitDTO response = new UnitDTO(newUnit);
                    return Ok(response);
                }
                catch (Exception ex)
                {
                    // Local log entry. Database reconciliation issues are more serious so reported as Error
                    Logger.Msg<UnitsController>($"[ADD] Database sync error {ex.Message}", Serilog.Events.LogEventLevel.Error);

                    // Return response to client
                    return StatusCode(500, new { errors = "Database update failed. Contact the administrator to resolve this issue." });
                }
            }

        }

        /// <summary>
        /// Modify an existing Unit record
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        [HttpPut("modify")]
        [Authorize(Policy = "StaffMember")]
        public async Task<ActionResult> ModifyUnit([FromBody] UnitDTO unit)
        {
            if (UnitExists(unit.UnitID))
            {
                // Call BLL Unit Modify method with all the parameters
                object BLLResponse = new UnitBLL(_context).ModifyUnitBLL(unit: unit);

                if (BLLResponse.GetType().BaseType == typeof(Exception))
                {
                    // Create log entries for Debug log
                    ((APIException)BLLResponse).Exceptions.ForEach(ex => Logger.Msg<UnitsController>((Exception)ex, Serilog.Events.LogEventLevel.Debug));

                    // Return response from API
                    return BadRequest(new { errors = ((APIException)BLLResponse).Exceptions.Select(x => x.Message).ToArray() });
                }
                else
                {
                    try
                    {
                        UnitDTO modUnit = (UnitDTO)BLLResponse;

                        // Find the existing record based on ID
                        Unit currentRecord = _context.Units.Where(x => x.UnitID == modUnit.UnitID).First();

                        // Modify the record
                        currentRecord.Description = modUnit.Description;

                        // Save changes
                        await _context.SaveChangesAsync();

                        Logger.Msg<UnitsController>($"[{User.FindFirstValue("email")}] [MODIFY] UnitID: {currentRecord.UnitID} successful", Serilog.Events.LogEventLevel.Information);

                        // Return modified record as a DTO
                        UnitDTO response = new UnitDTO(currentRecord);
                        return Ok(response);
                    }
                    catch (Exception ex)
                    {
                        // Local log entry. Database reconciliation issues are more serious so reported as Error
                        Logger.Msg<UnitsController>($"[MODIFY] Database sync error {ex.Message}", Serilog.Events.LogEventLevel.Error);

                        // Return response to client
                        return StatusCode(500, new { errors = "Database update failed. Contact the administrator to resolve this issue." });
                    }
                }
            }
            else
            {
                return NotFound();
            }
        } // End of ModifyUnit

        /// <summary>
        /// Deletes the Unit record, provided that there are no FK dependencies
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("delete")]
        [Authorize(Policy = "SuperAdmin")]
        public async Task<ActionResult> Delete(int id)
        {
            // Find existing Unit record in DB
            Unit unit = await _context.Units.FindAsync(id);

            if (unit == null)
            {
                Logger.Msg<UnitsController>($"[{User.FindFirstValue("email")}] [DELETE] UnitID: {id} not found", Serilog.Events.LogEventLevel.Debug);
                return NotFound();
            }

            try
            {
                _context.Units.Remove(unit);
                await _context.SaveChangesAsync();

                Logger.Msg<UnitsController>($"[{User.FindFirstValue("email")}] [DELETE] UnitID: {id} success", Serilog.Events.LogEventLevel.Information);
                return Ok(new UnitDTO(unit));
            }
            catch (Exception ex)
            {
                // Probably due to FK violation
                Logger.Msg<UnitsController>($"[DELETE] Database sync error {ex.Message}", Serilog.Events.LogEventLevel.Error);

                // Return response to client
                return StatusCode(500, new { errors = "Database update failed." });
            }

        }

        private bool UnitExists(int id)
        {
            return _context.Units.Any(e => e.UnitID == id);
        }
    }
}
