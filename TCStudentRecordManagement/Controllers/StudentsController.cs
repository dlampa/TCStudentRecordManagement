using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TCStudentRecordManagement.Utils;
using TCStudentRecordManagement.Models;
using TCStudentRecordManagement.Controllers.Exceptions;
using TCStudentRecordManagement.Models.DTO;
using TCStudentRecordManagement.Controllers.BLL;
using System.Security.Claims;

namespace TCStudentRecordManagement.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Policy = "StaffMember")]
    public class StudentsController : ControllerBase
    {
        private readonly DataContext _context;

        public StudentsController(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get basic student records from the database. Returns all students.
        /// </summary>
        /// <returns></returns>
        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<StudentDTO>>> List()
        {
            // Convert Student to StudentDTO
            List<Student> studentData = await _context.Students.ToListAsync();
            List<StudentDTO> result = new List<StudentDTO>();
            studentData.ForEach(x => result.Add(new StudentDTO(x)));

            // Log to debug log
            Logger.Msg<StudentsController>($"[{User.FindFirstValue("email")}] [LIST]", Serilog.Events.LogEventLevel.Debug);
            return result;

        } // End of List

        /// <summary>
        /// Gets detailed Student records, with optional cohort filter
        /// </summary>
        /// <param name="cohortID"></param>
        /// <returns></returns>
        [HttpGet("details")]
        public async Task<ActionResult<IEnumerable<StudentDetailDTO>>> Details(int cohortID)
        {
            // Call BLL method with all the parameters
            object BLLResponse = new StudentBLL(_context).StudentDetailBLL(cohortID: cohortID);

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
                    int BLLCohortID = (int)BLLResponse;

                    // Retrieve data
                    List<Student> studentData = await _context.Students.Where(x => x.CohortID == BLLCohortID).Include(student => student.CohortMember).Include(student => student.UserData).ToListAsync();

                    // Convert Student to StudentDetailDTO
                    List<StudentDetailDTO> result = new List<StudentDetailDTO>();
                    studentData.ForEach(x => result.Add(new StudentDetailDTO(x)));

                    // Log to debug log
                    Logger.Msg<StudentsController>($"[{User.FindFirstValue("email")}] [DETAILS]", Serilog.Events.LogEventLevel.Debug);
                    return result;
                }
                catch (Exception ex)
                {
                    // Local log entry. Database reconciliation issues are more serious so reported as Error
                    Logger.Msg<StudentsController>($"[DETAILS] Database read error {ex.Message}", Serilog.Events.LogEventLevel.Error);

                    // Return response to client
                    return StatusCode(500, new { errors = "Database read failed. Contact the administrator to resolve this issue." });
                }
            }

        } // End of Details

        /// <summary>
        /// Return Student record based on StudentID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
                Logger.Msg<StudentsController>($"[{User.FindFirstValue("email")}] [GET] UserID: {id}", Serilog.Events.LogEventLevel.Debug);
                return result;
            }
        } // End of GetStudent

        /// <summary>
        /// Add a Student record to the database
        /// </summary>
        /// <param name="firstname"></param>
        /// <param name="lastname"></param>
        /// <param name="email"></param>
        /// <param name="active"></param>
        /// <param name="cohortID"></param>
        /// <param name="bearTracksID"></param>
        /// <returns></returns>

        [HttpPut("add")]
        public async Task<ActionResult> AddStudent(string firstname, string lastname, string email, bool active, int cohortID, string bearTracksID)
        {
            // Figure out if the user is a Super Admin (important for override on active cohort)
            bool isSuperAdmin = User.Claims.Any(x => x.Type == System.Security.Claims.ClaimTypes.Role && x.Value == "SuperAdmin");

            // Call BLL Student Add method with all the parameters
            object BLLResponse = new StudentBLL(_context).AddStudentBLL(firstname: firstname, lastname: lastname, email: email, active: active, cohortID: cohortID, bearTracksID: bearTracksID, userIsSuperAdmin: isSuperAdmin);

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
                    Dictionary<string, object> BLLResponseDic = (Dictionary<string, object>)(BLLResponse);
                    int newStudentUserID;

                    // Check if the user already exists, but the student record doesn't
                    if (!(bool)BLLResponseDic["UserAlreadyInUsersTable"])
                    {
                        // Split the result from the BLL into relevant User and Student objects
                        User newUser = new User
                        {
                            Firstname = BLLResponseDic["Firstname"].ToString(),
                            Lastname = BLLResponseDic["Lastname"].ToString(),
                            Email = BLLResponseDic["Email"].ToString(),
                            Active = BLLResponseDic["Active"].ToString() == "True"
                        };

                        // Add to model and save changes
                        _context.Users.Add(newUser);
                        await _context.SaveChangesAsync();
                        newStudentUserID = newUser.UserID;
                    }
                    else
                    {
                        // User is already in the database
                        newStudentUserID = _context.Users.Where(x => x.Email == BLLResponseDic["Email"].ToString()).First().UserID;
                        Logger.Msg<StudentsController>($"[{User.FindFirstValue("email")}] [ADD] student '{BLLResponseDic["Email"].ToString()}' already in user table", Serilog.Events.LogEventLevel.Warning);
                    }

                    // Create the Student record
                    Student newStudent = new Student
                    {
                        UserID = newStudentUserID,
                        CohortID = (int)BLLResponseDic["CohortID"],
                        BearTracksID = BLLResponseDic["BearTracksID"]?.ToString()
                    };

                    // Add to model and save changes
                    _context.Students.Add(newStudent);
                    await _context.SaveChangesAsync();

                    Logger.Msg<StudentsController>($"[{User.FindFirstValue("email")}] [ADD] student '{BLLResponseDic["Email"].ToString()}' successful", Serilog.Events.LogEventLevel.Information);
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

        } // End of AddStudent

        /// <summary>
        /// Modify a Student record in the database
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        [HttpPut("modify")]
        public async Task<ActionResult> ModifyStudent(StudentModDTO student)
        {
            if (StudentExists(student.StudentID))
            {
                // Figure out if the user is a Super Admin (important for override on active cohort)
                bool isSuperAdmin = User.Claims.Any(x => x.Type == System.Security.Claims.ClaimTypes.Role && x.Value == "SuperAdmin");

                // Call BLL Student Modify method with all the parameters
                object BLLResponse = new StudentBLL(_context).ModifyStudentBLL(student: student, userIsSuperAdmin: isSuperAdmin);

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
                        StudentModDTO modifiedStudentData = (StudentModDTO)BLLResponse;

                        // Determine the UserID corresponding to the UserID

                        int studentUserID = _context.Students.Where(x => x.StudentID == modifiedStudentData.StudentID).First().UserID;

                        // Modify UserID record accordingly

                        User userRecord = _context.Users.Where(x => x.UserID == studentUserID).First();

                        userRecord.Firstname = modifiedStudentData.Firstname;
                        userRecord.Lastname = modifiedStudentData.Lastname;
                        userRecord.Email = modifiedStudentData.Email;
                        userRecord.Active = modifiedStudentData.Active;

                        // Save changes
                        await _context.SaveChangesAsync();

                        // Modify Student record
                        Student studentRecord = _context.Students.Where(x => x.StudentID == modifiedStudentData.StudentID).First();

                        studentRecord.CohortID = modifiedStudentData.CohortID;
                        studentRecord.BearTracksID = modifiedStudentData.BearTracksID;

                        // Save changes
                        await _context.SaveChangesAsync();

                        Logger.Msg<StudentsController>($"[{User.FindFirstValue("email")}] [MODIFY] student '{modifiedStudentData.Email}' successful", Serilog.Events.LogEventLevel.Information);
                        StudentDTO response = new StudentDTO(studentRecord);
                        return Ok(response);
                    }
                    catch (Exception ex)
                    {
                        // Local log entry. Database reconciliation issues are more serious so reported as Error
                        Logger.Msg<StudentsController>($"[MODIFY] Database sync error {ex.Message}", Serilog.Events.LogEventLevel.Error);

                        // Return response to client
                        return StatusCode(500, new { errors = "Database update failed. Contact the administrator to resolve this issue." });
                    }
                }
            }
            else
            {
                return NotFound();
            }

        } // End of ModifyStudent

        /// <summary>
        /// Activate/Deactivate a Student
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpPut("active")]
        public async Task<ActionResult> ChangeActiveState(int id, bool state)
        {
            Student student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                Logger.Msg<StudentsController>($"[{User.FindFirstValue("email")}] [ACTIVE] StudentID: {id} not found", Serilog.Events.LogEventLevel.Debug);
                return NotFound();
            }

            try
            {
                // Get the User table record corresponding to the student
                User userData = _context.Users.Where(x => x.UserID == student.UserID).First();

                // Change active bit to match parameter
                userData.Active = state;

                // Save to DB
                await _context.SaveChangesAsync();

                Logger.Msg<StudentsController>($"[{User.FindFirstValue("email")}] [ACTIVE] StudentID: {id} {(state ? "activated" : "deactivated")}", Serilog.Events.LogEventLevel.Information);
                return Ok(new UserDTO(userData));
            }
            catch (Exception ex)
            {
                Logger.Msg<StudentsController>($"[ACTIVE] Database sync error {ex.Message}", Serilog.Events.LogEventLevel.Error);

                // Return response to client
                return StatusCode(500, new { errors = "Database update failed. Contact the administrator" });
            }

        }// End of ChangeActiveState

        /// <summary>
        /// Delete a Student Record from the database. Requires SuperAdmin authorization level
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("delete")]
        [Authorize(Policy = "SuperAdmin")]
        public async Task<ActionResult> Delete(int id)
        {
            // Find existing Cohort record in DB
            Student student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                Logger.Msg<StudentsController>($"[{User.FindFirstValue("email")}] [DELETE] StudentID: {id} not found", Serilog.Events.LogEventLevel.Debug);
                return NotFound();
            }

            try
            {
                // This will fail if the student has other records which would be orphaned
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();

                Logger.Msg<StudentsController>($"[{User.FindFirstValue("email")}] [DELETE] StudentID: {id} success", Serilog.Events.LogEventLevel.Information);
                return Ok(new StudentDTO(student));
            }
            catch (Exception ex)
            {
                // Probably due to FK violation
                Logger.Msg<StudentsController>($"[DELETE] Database sync error {ex.Message}", Serilog.Events.LogEventLevel.Error);

                // Return response to client
                return StatusCode(500, new { errors = "Database update failed." });
            }

        } // End of Delete

        /// <summary>
        /// Checks for existence of Student record in the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.StudentID == id);
        }

    }

}
