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
            List<TaskType> taskTypeData = await _context.TaskTypes.ToListAsync();
            List<TaskTypeDTO> result = new List<TaskTypeDTO>();
            taskTypeData.ForEach(x => result.Add(new TaskTypeDTO(x)));

            // Log to debug log
            Logger.Msg<TaskTypesController>($"[{User.FindFirstValue("email")}] [LIST]", Serilog.Events.LogEventLevel.Debug);
            return result;

        } // End of List

        /// <summary>
        /// Add a TaskType record
        /// </summary>
        /// <param name="description"></param>
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

                    Logger.Msg<TaskTypesController>($"[{User.FindFirstValue("email")}] [ADD] TaskType '{description}' successful", Serilog.Events.LogEventLevel.Information);

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

        } // End of AddTaskType

        /// <summary>
        /// Modify an existing TaskType record
        /// </summary>
        /// <param name="taskType"></param>
        /// <returns></returns>
        [HttpPut("modify")]
        [Authorize(Policy = "StaffMember")]
        public async Task<ActionResult> ModifyTaskType([FromBody] TaskTypeDTO taskType)
        {
            if (TaskTypeExists(taskType.TypeID))
            {

                // Call BLL TaskType Modify method with all the parameters
                object BLLResponse = new TaskTypeBLL(_context).ModifyTaskTypeBLL(taskType: taskType);

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
                        TaskTypeDTO modTaskType = (TaskTypeDTO)BLLResponse;

                        // Find the existing record based on ID
                        TaskType currentRecord = _context.TaskTypes.Where(x => x.TypeID == modTaskType.TypeID).First();

                        // Modify the record
                        currentRecord.Description = modTaskType.Description;

                        // Save changes
                        await _context.SaveChangesAsync();

                        Logger.Msg<TaskTypesController>($"[{User.FindFirstValue("email")}] [MODIFY] TypeID: {currentRecord.TypeID} successful", Serilog.Events.LogEventLevel.Information);

                        // Return modified record as a DTO
                        TaskTypeDTO response = new TaskTypeDTO(currentRecord);
                        return Ok(response);
                    }
                    catch (Exception ex)
                    {
                        // Local log entry. Database reconciliation issues are more serious so reported as Error
                        Logger.Msg<TaskTypesController>($"[MODIFY] Database sync error {ex.Message}", Serilog.Events.LogEventLevel.Error);

                        // Return response to client
                        return StatusCode(500, new { errors = "Database update failed. Contact the administrator to resolve this issue." });
                    }
                }
            }
            else
            {
                return NotFound();
            }
        } // End of ModifyTaskType

        /// <summary>
        /// Deletes the TaskType record, provided that there are no FK dependencies
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("delete")]
        [Authorize(Policy = "StaffMember")]
        public async Task<ActionResult> Delete(int id)
        {
            // Find existing TaskType record in DB
            TaskType taskType = await _context.TaskTypes.FindAsync(id);

            if (taskType == null)
            {
                Logger.Msg<TaskTypesController>($"[{User.FindFirstValue("email")}] [DELETE] TypeID: {id} not found", Serilog.Events.LogEventLevel.Debug);
                return NotFound();
            }

            try
            {
                _context.TaskTypes.Remove(taskType);
                await _context.SaveChangesAsync();

                Logger.Msg<TaskTypesController>($"[{User.FindFirstValue("email")}] [DELETE] TypeID: {id} success", Serilog.Events.LogEventLevel.Information);
                return Ok(new TaskTypeDTO(taskType));
            }
            catch (Exception ex)
            {
                // Probably due to FK violation
                Logger.Msg<TaskTypesController>($"[DELETE] Database sync error {ex.Message}", Serilog.Events.LogEventLevel.Error);

                // Return response to client
                return StatusCode(500, new { errors = "Database update failed." });
            }

        } // End of Delete

        /// <summary>
        /// Checks for existence of TaskType records in the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool TaskTypeExists(int id)
        {
            return _context.TaskTypes.Any(e => e.TypeID == id);
        }
    }
}
