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
using TCStudentRecordManagement.Utils;


namespace TCStudentRecordManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class CohortsController : ControllerBase
    {
        private readonly DataContext _context;

        public CohortsController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Cohorts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cohort>>> All()
        {
            return await _context.Cohorts.ToListAsync();
        }

        // Using PUT for addition methods as PUT implies that resource will only be added once.
        // Ref: https://www.w3schools.com/tags/ref_httpmethods.asp
        [HttpPut]
        [Authorize(Policy = "StaffMember")]
        [Route("/cohorts/add")]
        public ActionResult AddCohort_Target(string name, string startDate, string endDate)
        {
            // This will by default check if there is Authorization to execute. If there isn't an authorization, then API server automatically
            // returns 401 response.

         
            // Check if there is an authorization

            // Call BLL Cohort Add method with all the parameters

            // Catch errors and return exceptions as appropriate
            Logger.Msg<CohortsController>($"[{User.Claims.Where(x => x.Type == "email").FirstOrDefault().Value}] [ADD] {name}", Serilog.Events.LogEventLevel.Information);

            
            return Ok(new { Name = name, StartDate = startDate, EndDate = endDate });
        }

        // GET: api/Cohorts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cohort>> GetCohort(int id)
        {
            var cohort = await _context.Cohorts.FindAsync(id);

            if (cohort == null)
            {
                return NotFound();
            }

            return cohort;
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
