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
    [Route("[Controller]")]
    [Authorize]
    [ApiController]
    public class TaskTypesController : ControllerBase
    {
        private readonly DataContext _context;

        public TaskTypesController(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get a list of all the records from the TaskTypes table
        /// </summary>
        /// <returns></returns>
        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<TaskTypeDTO>>> List()
        {

            // Convert TaskType to TaskTypeDTO
            List<TaskType> taskTypeData= await _context.TaskTypes.ToListAsync();
            List<TaskTypeDTO> result = new List<TaskTypeDTO>();
            taskTypeData.ForEach(x => result.Add(new TaskTypeDTO(x)));

            // Log to debug log
            Logger.Msg<TaskTypesController>($"[{User.FindFirstValue("email")}] [LIST]", Serilog.Events.LogEventLevel.Debug);
            return result;

        }
        
        /// <summary>
        /// Add a TaskType record to the TaskTypes table
        /// </summary>
        /// <param name="name"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpPut("add")]
        [Authorize(Policy = "StaffMember")]
        public async Task<ActionResult> AddTaskType(string description)
        {

            // Call BLL TaskType Add method with all the parameters
            object BLLResponse = new TaskTypeBLL(_context).AddTaskTypeBLL(description: description);

            // Get the base class for the response
            // Ref: https://docs.microsoft.com/en-us/dotnet/api/system.type.basetype?view=netcore-3.1
            if (BLLResponse.GetType().BaseType == typeof(Exception))
            {
                // Create log entries for Debug log
                ((APIException)BLLResponse).Exceptions.ForEach(ex => Logger.Msg<TaskTypesController>((Exception)ex, Serilog.Events.LogEventLevel.Debug));

                // Return response from API
                return BadRequest(new { errors = ((APIException)BLLResponse).Exceptions.Select(x => x.Message).ToArray() });
            }
            else
            {
                try
                {
                    TaskType newTaskType = new TaskType { Description = ((TaskTypeDTO)BLLResponse).Description };

                    // Create the record
                    _context.TaskTypes.Add(newTaskType);
                    await _context.SaveChangesAsync();

                    Logger.Msg<CohortsController>($"[{User.FindFirstValue("email")}] [ADD] TaskType '{description}' successful", Serilog.Events.LogEventLevel.Information);

                    // Convert back to DTO and return to user
                    TaskTypeDTO response = new TaskTypeDTO(newTaskType);
                    return Ok(response);
                }
                catch (Exception ex)
                {
                    // Local log entry. Database reconciliation issues are more serious so reported as Error
                    Logger.Msg<TaskTypesController>($"[ADD] Database sync error {ex.Message}", Serilog.Events.LogEventLevel.Error);

                    // Return response to client
                    return StatusCode(500, new { errors = "Database update failed. Contact the administrator to resolve this issue." });
                }
            }

        }
/*
        // PUT: Modify Cohort [CohortBLL] [Return DTO]
        [HttpPut("modify")]
        [Authorize(Policy = "StaffMember")]
        public async Task<ActionResult> ModifyCohort_Target([FromBody] CohortDTO cohort)
        {

            if (CohortExists(cohort.CohortID))
            {

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
*/

        private bool TaskTypeExists(int id)
        {
            return _context.TaskTypes.Any(e => e.TypeID == id);
        }
    }
}
