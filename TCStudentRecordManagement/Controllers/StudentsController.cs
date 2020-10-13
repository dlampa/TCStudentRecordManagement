using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TCStudentRecordManagement.Utils;
using TCStudentRecordManagement.Models;
using TCStudentRecordManagement.Controllers.Exceptions;
using TCStudentRecordManagement.Controllers.DTO;
using TCStudentRecordManagement.Controllers.BLL;


namespace TCStudentRecordManagement.Controllers
{
    [Route("/students")]
    [Authorize(Policy = "StaffMember")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly DataContext _context;

        public StudentsController(DataContext context)
        {
            _context = context;
        }

        // GET: All Students [NO BLL] [Return DTO]
        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<StudentDTO>>> List()
        {
            // Convert Student to StudentDTO
            List<Student> studentData = await _context.Students.ToListAsync();
            List<StudentDTO> result = new List<StudentDTO>();
            studentData.ForEach(x => result.Add(new StudentDTO(x)));

            // Log to debug log
            Logger.Msg<StudentsController>($"[{User.Claims.Where(x => x.Type == "email").FirstOrDefault().Value}] [LIST]", Serilog.Events.LogEventLevel.Debug);
            return result;
        }
        
        // GET: All Students with Cohort and User detail [NO BLL] [Return DTO]
        [HttpGet("details")]
        public async Task<ActionResult<IEnumerable<StudentDetailDTO>>> Details()
        {
            // Convert Student to StudentDetailDTO
            List<Student> studentData = await _context.Students.ToListAsync();
            studentData.ForEach(x => x.CohortMember = _context.Cohorts.Where(y => y.CohortID == x.CohortID).FirstOrDefault());
            studentData.ForEach(x => x.UserData = _context.Users.Where(y => y.UserID == x.UserID).FirstOrDefault());
            List<StudentDetailDTO> result = new List<StudentDetailDTO>();
            studentData.ForEach(x => result.Add(new StudentDetailDTO(x)));

            // Log to debug log
            Logger.Msg<StudentsController>($"[{User.Claims.Where(x => x.Type == "email").FirstOrDefault().Value}] [DETAILS]", Serilog.Events.LogEventLevel.Debug);
            return result;
        }

        // GET: Specific Student based on StudentID [NO BLL] [Return DTO]
        [HttpGet("get")]
        public async Task<ActionResult<StudentDTO>> GetStudent(int id)
        {
            Student student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }
            else
            {
                // Convert to DTO
                StudentDTO result = new StudentDTO(student);

                // Log to debug log
                Logger.Msg<StudentsController>($"[{User.Claims.Where(x => x.Type == "email").FirstOrDefault().Value}] [GET] UserID: {id}", Serilog.Events.LogEventLevel.Debug);
                return result;
            }
        }

        // PUT: Add Cohort [StudentBLL] [Return DTO]

        [HttpPut("add")]
        public async Task<ActionResult> AddStudent_Target(string firstname, string lastname, string email, bool active, int cohortID, string bearTracksID)
        {
            // Figure out if the user is a Super Admin (important for override on active cohort
            bool isSuperAdmin = User.Claims.Any(x => x.Type == System.Security.Claims.ClaimTypes.Role && x.Value == "SuperAdmin");

            // Call BLL Student Add method with all the parameters
            object BLLResponse = new StudentBLL(_context).AddStudent(firstname: firstname, lastname: lastname, email: email, active: active, cohortID: cohortID, bearTracksID: bearTracksID, userIsSuperAdmin: isSuperAdmin);

            if (BLLResponse.GetType().BaseType == typeof(Exception))
            {
                // Create log entries for Debug log
                ((APIException)BLLResponse).Exceptions.ForEach(ex => Logger.Msg<StudentsController>((Exception)ex, Serilog.Events.LogEventLevel.Debug));

                // Return response from API
                return BadRequest(new { errors = ((APIException)BLLResponse).Exceptions.Select(x => x.Message).ToArray() });
            }
            else
            {
                try
                {
                    // Split the result from the BLL into relevant User and Student objects
                    User newUser = new User
                    {
                        Firstname = BLLResponse?.GetType().GetProperty("Firstname")?.GetValue(BLLResponse, null).ToString(),
                        Lastname = BLLResponse?.GetType().GetProperty("Lastname")?.GetValue(BLLResponse, null).ToString(),
                        Email = BLLResponse?.GetType().GetProperty("Email")?.GetValue(BLLResponse, null).ToString(),
                        Active = BLLResponse?.GetType().GetProperty("Active")?.GetValue(BLLResponse, null).ToString() == "True"
                    };

                    // Add to model and save changes
                    _context.Users.Add(newUser);
                    await _context.SaveChangesAsync();

                    // Create the Student record
                    Student newStudent = new Student
                    {
                        UserID = newUser.UserID,
                        CohortID = (int)BLLResponse?.GetType().GetProperty("CohortID")?.GetValue(BLLResponse, null),
                        BearTracksID = BLLResponse?.GetType().GetProperty("BearTracksID")?.GetValue(BLLResponse, null)?.ToString()
                    };

                    // Add to model and save changes
                    _context.Students.Add(newStudent);
                    await _context.SaveChangesAsync();

                    Logger.Msg<StudentsController>($"[{User.Claims.Where(x => x.Type == "email").FirstOrDefault().Value}] [ADD] student '{newUser.Email}' successful", Serilog.Events.LogEventLevel.Information);
                    StudentDetailDTO response = new StudentDetailDTO(newStudent);
                    return Ok(response);
                }
                catch (Exception ex)
                {
                    // Local log entry. Database reconciliation issues are more serious so reported as Error
                    Logger.Msg<StudentsController>($"[ADD] Database sync error {ex.Message}", Serilog.Events.LogEventLevel.Error);

                    // Return response to client
                    return StatusCode(500, new { errors = "Database update failed. Contact the administrator to resolve this issue." });
                }
            }

        }
        // PUT: api/Students/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(int id, Student student)
        {
            if (id != student.StudentID)
            {
                return BadRequest();
            }

            _context.Entry(student).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
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

        // POST: api/Students
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Student>> PostStudent(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStudent", new { id = student.StudentID }, student);
        }

        // DELETE: api/Students/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Student>> DeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return student;
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.StudentID == id);
        }
    }
}
