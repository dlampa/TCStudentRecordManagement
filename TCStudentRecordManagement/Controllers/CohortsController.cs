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
    [Route("/cohorts")]
    [ApiController]
    public partial class CohortsController : ControllerBase
    {
        private readonly DataContext _context;

        public CohortsController(DataContext context)
        {
            _context = context;
        }

        // GET: All Cohorts [NO BLL] 
        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<Cohort>>> List()
        {
            return await _context.Cohorts.ToListAsync();
        }

        // GET: Cohort by CohortID [NO BLL]
        [HttpGet]
        [Route("/cohorts/get")]
        public async Task<ActionResult<Cohort>> Get(int id)
        {
            var cohort = await _context.Cohorts.FindAsync(id);

            if (cohort == null)
            {
                return NotFound();
            }

            return cohort;
        }

        // Using PUT for addition methods as PUT implies that resource will only be added once.
        // Ref: https://www.w3schools.com/tags/ref_httpmethods.asp
        [HttpPut]
        [Authorize(Policy = "StaffMember")]
        [Route("/cohorts/add")]
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
                ((APIException)BLLResponse).Exceptions.ForEach(ex => Logger.Msg<CohortsController>((Exception)ex, Serilog.Events.LogEventLevel.Debug) );

                // Return response from API
                return BadRequest(new { errors = ((APIException)BLLResponse).Exceptions.Select(x => x.Message).ToArray() });
            } 
            else
            {
                try
                {
                    _context.Cohorts.Add((Cohort)BLLResponse);
                    await _context.SaveChangesAsync();
                    Logger.Msg<CohortsController>($"[{User.Claims.Where(x => x.Type == "email").FirstOrDefault().Value}] [ADD] {name} successful", Serilog.Events.LogEventLevel.Information);
                    return Ok(BLLResponse);
                }
                catch (Exception ex)
                {
                    // Local log entry. Database reconciliation issues are more serious so reported as Error
                    Logger.Msg<CohortsController>($"[ADD] Database sync error {ex.Message}", Serilog.Events.LogEventLevel.Error);

                    // Return response to client
                    return StatusCode(500, new { errors = "Database update failed. Contact the administrator to resolve this issue."});
                }
            }

        }

        

        // PUT: api/Cohorts/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCohort(int id, Cohort cohort)
        {
            if (id != cohort.CohortID)
            {
                return BadRequest();
            }

            _context.Entry(cohort).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CohortExists(id))
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

        // POST: api/Cohorts
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Cohort>> PostCohort(Cohort cohort)
        {
            _context.Cohorts.Add(cohort);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCohort", new { id = cohort.CohortID }, cohort);
        }

        // DELETE: api/Cohorts/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Cohort>> DeleteCohort(int id)
        {
            var cohort = await _context.Cohorts.FindAsync(id);
            if (cohort == null)
            {
                return NotFound();
            }

            _context.Cohorts.Remove(cohort);
            await _context.SaveChangesAsync();

            return cohort;
        }

        private bool CohortExists(int id)
        {
            return _context.Cohorts.Any(e => e.CohortID == id);
        }
    }
}
