using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        // GET: api/TaskTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskType>> GetTaskType(int id)
        {
            var taskType = await _context.TaskTypes.FindAsync(id);

            if (taskType == null)
            {
                return NotFound();
            }

            return taskType;
        }

        // PUT: api/TaskTypes/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTaskType(int id, TaskType taskType)
        {
            if (id != taskType.TypeID)
            {
                return BadRequest();
            }

            _context.Entry(taskType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskTypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/TaskTypes
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<TaskType>> PostTaskType(TaskType taskType)
        {
            _context.TaskTypes.Add(taskType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTaskType", new { id = taskType.TypeID }, taskType);
        }

        // DELETE: api/TaskTypes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TaskType>> DeleteTaskType(int id)
        {
            var taskType = await _context.TaskTypes.FindAsync(id);
            if (taskType == null)
            {
                return NotFound();
            }

            _context.TaskTypes.Remove(taskType);
            await _context.SaveChangesAsync();

            return taskType;
        }

        private bool TaskTypeExists(int id)
        {
            return _context.TaskTypes.Any(e => e.TypeID == id);
        }
    }
}
