using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Connections.Features;
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
    public class TasksController : ControllerBase
    {
        private readonly DataContext _context;

        public TasksController(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets the Task records based on input criteria. Students are limited to viewing assignments from their own cohort.
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="cohortID"></param>
        /// <param name="typeID"></param>
        /// <param name="unitID"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet("get")]
        public async Task<ActionResult<IEnumerable<Models.Task>>> Get(int taskID, int cohortID, int typeID, int unitID, DateTime startDate, DateTime endDate)
        {
            // Call GetAttendanceBLL method with all the parameters
            object BLLResponse = new TaskBLL(_context).GetTaskBLL(taskID: taskID, cohortID: cohortID, typeID: typeID, unitID: unitID, startDate: startDate, endDate: endDate, userClaims: User);

            // Get the base class for the response
            // Ref: https://docs.microsoft.com/en-us/dotnet/api/system.type.basetype?view=netcore-3.1
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
                    
                    TaskDTO BLLResponseDTO = (TaskDTO)(BLLResponse);

                    // Apply all the criteria with supplied or default values from BLL
                    IQueryable<Models.Task> dbRequest = _context.Tasks
                        .Where(x => x.StartDate >= BLLResponseDTO.StartDate && x.EndDate <= BLLResponseDTO.EndDate);

                    if (BLLResponseDTO.CohortID != 0) dbRequest = dbRequest.Where(x => x.CohortID == BLLResponseDTO.CohortID);
                    if (BLLResponseDTO.TypeID != 0) dbRequest = dbRequest.Where(x => x.TypeID == BLLResponseDTO.TypeID);
                    if (BLLResponseDTO.UnitID != 0) dbRequest = dbRequest.Where(x => x.UnitID == BLLResponseDTO.UnitID);
                    if (BLLResponseDTO.TaskID != 0) dbRequest = dbRequest.Where(x => x.TaskID == BLLResponseDTO.TaskID);
                    
                    List<Models.Task> dbResponse = await dbRequest.ToListAsync();

                    // Convert result to TaskDTO
                    List<TaskDTO> response = new List<TaskDTO>();
                    dbResponse.ForEach(x => response.Add(new TaskDTO(x)));

                    Logger.Msg<TasksController>($"[{User.FindFirstValue("email")}] [GET] ", Serilog.Events.LogEventLevel.Debug);
                    return Ok(response);
                }
                catch (Exception ex)
                {
                    // Local log entry. Database reconciliation issues are more serious so reported as Error
                    Logger.Msg<TasksController>($"[GET] Database sync error {ex.Message}", Serilog.Events.LogEventLevel.Error);

                    // Return response to client
                    return StatusCode(500, new { errors = "Database update failed. Contact the administrator to resolve this issue." });
                }
            }
        }// End of Get

        /// <summary>
        /// Add a new Task record to the Tasks table
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        [Route("add")]
        [Authorize(Policy = "StaffMember")]
        public async Task<ActionResult> AddTask(TaskDTO task)
        {

            // Call BLL Task Add method with all the parameters
            object BLLResponse = new TaskBLL(_context).AddTaskBLL(task: task);

            if (BLLResponse.GetType().BaseType == typeof(Exception))
            {
                // Create log entries for Debug log
                ((APIException)BLLResponse).Exceptions.ForEach(ex => Logger.Msg<TasksController>((Exception)ex, Serilog.Events.LogEventLevel.Debug));

                // Return response from API
                return BadRequest(new { errors = ((APIException)BLLResponse).Exceptions.Select(x => x.Message).ToArray() });
            }
            else
            {
                try
                {
                    TaskDTO newTaskDTO = (TaskDTO)BLLResponse;

                    Models.Task newTask = new Models.Task
                    {
                        UnitID = newTaskDTO.UnitID,
                        CohortID = newTaskDTO.CohortID,
                        TypeID = newTaskDTO.TypeID,
                        Title = newTaskDTO.Title,
                        StartDate = newTaskDTO.StartDate,
                        EndDate = newTaskDTO.EndDate,
                        DocURL = newTaskDTO.DocURL
                    };

                    _context.Tasks.Add(newTask);
                    await _context.SaveChangesAsync();

                    Logger.Msg<TasksController>($"[{User.FindFirstValue("email")}] [ADD] TaskID '{newTask.TaskID}' successful", Serilog.Events.LogEventLevel.Information);

                    // Convert back to DTO and return to user
                    TaskDTO response = new TaskDTO(newTask);
                    return Ok(response);
                }
                catch (Exception ex)
                {
                    // Local log entry. Database reconciliation issues are more serious so reported as Error
                    Logger.Msg<TasksController>($"[ADD] Database sync error {ex.Message}", Serilog.Events.LogEventLevel.Error);

                    // Return response to client
                    return StatusCode(500, new { errors = "Database update failed. Contact the administrator to resolve this issue." });
                }
            }

        } // End of AddTask

        /// <summary>
        /// Modify an existing TaskType record
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        [HttpPut("modify")]
        [Authorize(Policy = "StaffMember")]
        public async Task<ActionResult> ModifyTask([FromBody] TaskDTO task)
        {
            if (TaskExists(task.TypeID))
            {

                // Call BLL Task Modify method with all the parameters
                object BLLResponse = new TaskBLL(_context).ModifyTaskBLL(task: task);

                if (BLLResponse.GetType().BaseType == typeof(Exception))
                {
                    // Create log entries for Debug log
                    ((APIException)BLLResponse).Exceptions.ForEach(ex => Logger.Msg<TasksController>((Exception)ex, Serilog.Events.LogEventLevel.Debug));

                    // Return response from API
                    return BadRequest(new { errors = ((APIException)BLLResponse).Exceptions.Select(x => x.Message).ToArray() });
                }
                else
                {
                    try
                    {
                        TaskDTO modTask = (TaskDTO)BLLResponse;

                        // Find the existing record based on ID
                        Models.Task currentRecord = _context.Tasks.Where(x => x.TaskID == modTask.TaskID).First();

                        // Modify the record
                        currentRecord.UnitID = modTask.UnitID;
                        currentRecord.CohortID = modTask.CohortID;
                        currentRecord.TypeID = modTask.TypeID;
                        currentRecord.Title = modTask.Title;
                        currentRecord.StartDate = modTask.StartDate;
                        currentRecord.EndDate = modTask.EndDate;
                        currentRecord.DocURL = modTask.DocURL;

                        // Save changes
                        await _context.SaveChangesAsync();

                        Logger.Msg<TasksController>($"[{User.FindFirstValue("email")}] [MODIFY] TaskID: {currentRecord.TaskID} successful", Serilog.Events.LogEventLevel.Information);

                        // Return modified record as a DTO
                        TaskDTO response = new TaskDTO(currentRecord);
                        return Ok(response);
                    }
                    catch (Exception ex)
                    {
                        // Local log entry. Database reconciliation issues are more serious so reported as Error
                        Logger.Msg<TasksController>($"[MODIFY] Database sync error {ex.Message}", Serilog.Events.LogEventLevel.Error);

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
        /// Deletes the Task record, provided that there are no FK dependencies
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("delete")]
        [Authorize(Policy = "StaffMember")]
        public async Task<ActionResult> Delete(int id)
        {
            // Find existing TaskType record in DB
            Models.Task task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                Logger.Msg<TasksController>($"[{User.FindFirstValue("email")}] [DELETE] TaskID: {id} not found", Serilog.Events.LogEventLevel.Debug);
                return NotFound();
            }

            try
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();

                Logger.Msg<TasksController>($"[{User.FindFirstValue("email")}] [DELETE] TaskID: {id} success", Serilog.Events.LogEventLevel.Information);
                return Ok(new TaskDTO(task));
            }
            catch (Exception ex)
            {
                // Probably due to FK violation
                Logger.Msg<TasksController>($"[DELETE] Database sync error {ex.Message}", Serilog.Events.LogEventLevel.Error);

                // Return response to client
                return StatusCode(500, new { errors = "Database update failed. Perhaps there are students in this cohort?" });
            }

        }

        private bool TaskExists(int id)
        {
            return _context.Tasks.Any(e => e.TaskID == id);
        }



    }



}
