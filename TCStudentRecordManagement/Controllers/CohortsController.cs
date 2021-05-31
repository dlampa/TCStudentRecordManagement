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
using TCStudentRecordManagement.Models.DTO;
using TCStudentRecordManagement.Models;
using TCStudentRecordManagement.Utils;


namespace TCStudentRecordManagement.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class CohortsController : ControllerBase
    {
        private readonly DataContext _context;

        public CohortsController(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get a list of Cohorts from the database [GET]
        /// </summary>
        /// <param name="activeOnly"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CohortDTO>>> List(bool activeOnly = false, int id = -1)
        {
            // Convert Cohort to CohortDTO
            // If activeOnly is selected, return only active Cohorts
            
            List<Cohort> cohortData = activeOnly ? await _context.Cohorts.Where(x => DateTime.Now >= x.StartDate && DateTime.Now <= x.EndDate).ToListAsync() : await _context.Cohorts.ToListAsync();
            
            // Filter by id if selected
            if (id > -1)
            {
                cohortData.Where(x => x.CohortID == id).ToList();
            }

            List<CohortDTO> result = new List<CohortDTO>();
            cohortData.ForEach(x => result.Add(new CohortDTO(x)));

            // Log to debug log
            Logger.Msg<CohortsController>($"[{User.FindFirstValue("email")}] [LIST] {(activeOnly ? "active" : string.Empty)}{(id > -1 ? "CohortID: {id}" : string.Empty)}", Serilog.Events.LogEventLevel.Debug);
            return result;
        } // End of List

        /// <summary>
        /// Create a Cohort record in the database [POST]
        /// </summary>
        /// <param name="name"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Policy = "StaffMember")]
        public async Task<ActionResult> AddCohort(string name, DateTime startDate, DateTime endDate)
        {
            // This will by default check if there is Authorization to execute. If there isn't an authorization, then API server automatically
            // returns 401 response.

            // Call BLL Cohort Add method with all the parameters
            object BLLResponse = new CohortBLL(_context).AddCohortBLL(name: name, startDate: startDate, endDate: endDate);

            // Get the base class for the response
            // Ref: https://docs.microsoft.com/en-us/dotnet/api/system.type.basetype?view=netcore-3.1
            if (BLLResponse.GetType().BaseType == typeof(Exception))
            {
                // Create log entries for Debug log
                ((APIException)BLLResponse).Exceptions.ForEach(ex => Logger.Msg<CohortsController>((Exception)ex, Serilog.Events.LogEventLevel.Debug));

                // Return response from API
                return BadRequest(new { errors = ((APIException)BLLResponse).Exceptions.Select(x => x.Message).ToArray() });
            }
            else
            {
                try
                {
                    _context.Cohorts.Add((Cohort)BLLResponse);

                    await _context.SaveChangesAsync();

                    Logger.Msg<CohortsController>($"[{User.FindFirstValue("email")}] [ADD] '{name}' successful", Serilog.Events.LogEventLevel.Information);
                    CohortDTO response = new CohortDTO((Cohort)BLLResponse);
                    return Ok(response);
                }
                catch (Exception ex)
                {
                    // Local log entry. Database reconciliation issues are more serious so reported as Error
                    Logger.Msg<CohortsController>($"[ADD] Database sync error {ex.Message}", Serilog.Events.LogEventLevel.Error);

                    // Return response to client
                    return StatusCode(500, new { errors = "Database update failed. Contact the administrator to resolve this issue." });
                }
            }

        } // End of AddCohort

        /// <summary>
        /// Modify a Cohort record in the database
        /// </summary>
        /// <param name="cohort"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Policy = "StaffMember")]
        public async Task<ActionResult> ModifyCohort([FromBody] CohortDTO cohort)
        {

            if (CohortExists(cohort.CohortID))
            {

                // Call BLL Cohort Add method with all the parameters
                object BLLResponse = new CohortBLL(_context).ModifyCohortBLL(cohort: cohort);

                // Get the base class for the response
                // Ref: https://docs.microsoft.com/en-us/dotnet/api/system.type.basetype?view=netcore-3.1
                if (BLLResponse.GetType().BaseType == typeof(Exception))
                {
                    // Create log entries for Debug log
                    ((APIException)BLLResponse).Exceptions.ForEach(ex => Logger.Msg<CohortsController>((Exception)ex, Serilog.Events.LogEventLevel.Debug));

                    // Return response from API
                    return BadRequest(new { errors = ((APIException)BLLResponse).Exceptions.Select(x => x.Message).ToArray() });
                }
                else
                {
                    try
                    {
                        // Find the existing record based on ID
                        Cohort currentRecord = _context.Cohorts.Where(x => x.CohortID == cohort.CohortID).First();

                        // Modify the record
                        currentRecord.Name = ((Cohort)BLLResponse).Name;
                        currentRecord.StartDate = ((Cohort)BLLResponse).StartDate;
                        currentRecord.EndDate = ((Cohort)BLLResponse).EndDate;

                        // Save changes
                        await _context.SaveChangesAsync();

                        Logger.Msg<CohortsController>($"[{User.FindFirstValue("email")}] [MODIFY] CohortID: {currentRecord.CohortID} successful", Serilog.Events.LogEventLevel.Information);

                        // Return modified record as a DTO
                        CohortDTO response = new CohortDTO(currentRecord);
                        return Ok(response);
                    }
                    catch (Exception ex)
                    {
                        // Local log entry. Database reconciliation issues are more serious so reported as Error
                        Logger.Msg<CohortsController>($"[MODIFY] Database sync error {ex.Message}", Serilog.Events.LogEventLevel.Error);

                        // Return response to client
                        return StatusCode(500, new { errors = "Database update failed. Contact the administrator to resolve this issue." });
                    }
                }
            }
            else
            {
                return NotFound();
            }
        } // End of ModifyCohort

        /// <summary>
        /// Delete a Cohort Record from the database. Available only to SuperAdmin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(Policy = "SuperAdmin")]
        public async Task<ActionResult> Delete(int id)
        {
            // Find existing Cohort record in DB
            Cohort cohort = await _context.Cohorts.FindAsync(id);

            if (cohort == null)
            {
                Logger.Msg<CohortsController>($"[{User.FindFirstValue("email")}] [DELETE] CohortID: {id} not found", Serilog.Events.LogEventLevel.Debug);
                return NotFound();
            }

            try
            {
                _context.Cohorts.Remove(cohort);
                await _context.SaveChangesAsync();

                Logger.Msg<CohortsController>($"[{User.FindFirstValue("email")}] [DELETE] CohortID: {id} success", Serilog.Events.LogEventLevel.Information);
                return Ok(new CohortDTO(cohort));
            }
            catch (Exception ex)
            {
                // Probably due to FK violation
                Logger.Msg<CohortsController>($"[DELETE] Database sync error {ex.Message}", Serilog.Events.LogEventLevel.Error);

                // Return response to client
                return StatusCode(500, new { errors = "Database update failed. Perhaps there are students in this cohort?" });
            }

        } // End of Delete

        /// <summary>
        /// Checks for existence of Cohort record in the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool CohortExists(int id)
        {
            return _context.Cohorts.Any(e => e.CohortID == id);
        }
    }
}
