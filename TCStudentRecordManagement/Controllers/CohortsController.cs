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
using TCStudentRecordManagement.Controllers.DTO;
using TCStudentRecordManagement.Models;
using TCStudentRecordManagement.Utils;


namespace TCStudentRecordManagement.Controllers
{
    [Authorize(Policy = "StaffMember")]
    [Route("/cohorts")]
    [ApiController]
    public partial class CohortsController : ControllerBase
    {
        private readonly DataContext _context;

        public CohortsController(DataContext context)
        {
            _context = context;
        }

        // GET: All Cohorts [NO BLL] [Return DTO]
        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<CohortDTO>>> List()
        {
            // Convert Cohort to CohortDTO
            List<Cohort> cohortData = await _context.Cohorts.ToListAsync();
            List<CohortDTO> result = new List<CohortDTO>();
            cohortData.ForEach(x => result.Add(new CohortDTO(x)));

            // Log to debug log
            Logger.Msg<CohortsController>($"[{User.Claims.Where(x => x.Type == "email").FirstOrDefault().Value}] [LIST]", Serilog.Events.LogEventLevel.Debug);
            return result;
        }

        // GET: Cohort by CohortID [NO BLL] [Return DTO]
        [HttpGet("get")]
        public async Task<ActionResult<CohortDTO>> Get(int id)
        {
            Cohort cohort = await _context.Cohorts.FindAsync(id);

            if (cohort == null)
            {
                return NotFound();
            }
            else
            {
                // Convert to DTO
                CohortDTO result = new CohortDTO(cohort);

                // Log to debug log
                Logger.Msg<CohortsController>($"[{User.Claims.Where(x => x.Type == "email").FirstOrDefault().Value}] [GET] CohortID: {id}", Serilog.Events.LogEventLevel.Debug);
                return result;
            }
        }

        // PUT: Add Cohort [CohortBLL] [Return DTO]

        // Using PUT for addition methods as PUT implies that resource will only be added once.
        // Ref: https://www.w3schools.com/tags/ref_httpmethods.asp
        [HttpPut("add")]
        [Authorize(Policy = "StaffMember")]
        public async Task<ActionResult> AddCohort_Target(string name, DateTime startDate, DateTime endDate)
        {
            // This will by default check if there is Authorization to execute. If there isn't an authorization, then API server automatically
            // returns 401 response.

            // Call BLL Cohort Add method with all the parameters
            object BLLResponse = new CohortBLL(_context).AddCohort(name: name, startDate: startDate, endDate: endDate);

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
                    Logger.Msg<CohortsController>($"[{User.Claims.Where(x => x.Type == "email").FirstOrDefault().Value}] [ADD] '{name}' successful", Serilog.Events.LogEventLevel.Information);
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

        }

        // PUT: Modify Cohort [CohortBLL] [Return DTO]
        [HttpPut("modify")]
        [Authorize(Policy = "StaffMember")]
        public async Task<ActionResult> ModifyCohort_Target([FromBody] CohortDTO cohort)
        {

            if (CohortExists(cohort.CohortID))
            {
                // This will by default check if there is Authorization to execute. If there isn't an authorization, then API server automatically
                // returns 401 response.

                // Call BLL Cohort Add method with all the parameters
                object BLLResponse = new CohortBLL(_context).ModifyCohort(cohort: cohort);

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

                        Logger.Msg<CohortsController>($"[{User.Claims.Where(x => x.Type == "email").FirstOrDefault().Value}] [MODIFY] CohortID: {cohort.CohortID} successful", Serilog.Events.LogEventLevel.Information);

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
        } // End of ModifyCohort_Target

        // DELETE: Delete Cohort [NO BLL] [Return STATUS]
        [HttpDelete("delete")]
        [Authorize(Policy = "SuperAdmin")]
        public async Task<ActionResult> Delete(int id)
        {
            // Find existing Cohort record in DB
            Cohort cohort = await _context.Cohorts.FindAsync(id);

            if (cohort == null)
            {
                Logger.Msg<CohortsController>($"[{User.Claims.Where(x => x.Type == "email").FirstOrDefault().Value}] [DELETE] CohortID: {id} not found", Serilog.Events.LogEventLevel.Debug);
                return NotFound();
            }

            try
            {
                _context.Cohorts.Remove(cohort);
                await _context.SaveChangesAsync();

                Logger.Msg<CohortsController>($"[{User.Claims.Where(x => x.Type == "email").FirstOrDefault().Value}] [DELETE] CohortID: {id} success", Serilog.Events.LogEventLevel.Information);
                return Ok(new CohortDTO(cohort));
            }
            catch (Exception ex)
            {
                // Probably due to FK violation
                Logger.Msg<CohortsController>($"[DELETE] Database sync error {ex.Message}", Serilog.Events.LogEventLevel.Error);

                // Return response to client
                return StatusCode(500, new { errors = "Database update failed. Perhaps there are students in this cohort?" });
            }

        }

        private bool CohortExists(int id)
        {
            return _context.Cohorts.Any(e => e.CohortID == id);
        }
    }
}
