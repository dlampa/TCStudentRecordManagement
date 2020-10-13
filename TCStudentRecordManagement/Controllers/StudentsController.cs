﻿using System;
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

        // PUT: Add Student [StudentBLL] [Return DTO]

        [HttpPut("add")]
        public async Task<ActionResult> AddStudent_Target(string firstname, string lastname, string email, bool active, int cohortID, string bearTracksID)
        {
            // Figure out if the user is a Super Admin (important for override on active cohort)
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
                        Logger.Msg<StudentsController>($"[{User.Claims.Where(x => x.Type == "email").FirstOrDefault().Value}] [ADD] student '{BLLResponseDic["Email"].ToString()}' already in user table", Serilog.Events.LogEventLevel.Warning);
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

                    Logger.Msg<StudentsController>($"[{User.Claims.Where(x => x.Type == "email").FirstOrDefault().Value}] [ADD] student '{BLLResponseDic["Email"].ToString()}' successful", Serilog.Events.LogEventLevel.Information);
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

        // PUT: Modify Student [StudentBLL] [Return DTO]

        [HttpPut("modify")]
        public async Task<ActionResult> ModifyStudent_Target(string firstname, string lastname, string email, bool active, int cohortID, string bearTracksID)
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
                        Logger.Msg<StudentsController>($"[{User.Claims.Where(x => x.Type == "email").FirstOrDefault().Value}] [ADD] student '{BLLResponseDic["Email"].ToString()}' already in user table", Serilog.Events.LogEventLevel.Warning);
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

                    Logger.Msg<StudentsController>($"[{User.Claims.Where(x => x.Type == "email").FirstOrDefault().Value}] [ADD] student '{BLLResponseDic["Email"].ToString()}' successful", Serilog.Events.LogEventLevel.Information);
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

        // PUT: Activate/Deactivate Student [NO BLL] [Return DTO]
        [HttpPut("active")]
        public async Task<ActionResult> ChangeActiveState(int id, bool state)
        {
            Student student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                Logger.Msg<StudentsController>($"[{User.Claims.Where(x => x.Type == "email").FirstOrDefault().Value}] [ACTIVE] StudentID: {id} not found", Serilog.Events.LogEventLevel.Debug);
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

                Logger.Msg<StudentsController>($"[{User.Claims.Where(x => x.Type == "email").FirstOrDefault().Value}] [ACTIVE] StudentID: {id} {(state ? "activated": "deactivated")}", Serilog.Events.LogEventLevel.Information);
                return Ok(new UserDTO(userData));
            }
            catch (Exception ex)
            {
                // Probably due to FK violation
                Logger.Msg<StudentsController>($"[ACTIVE] Database sync error {ex.Message}", Serilog.Events.LogEventLevel.Error);

                // Return response to client
                return StatusCode(500, new { errors = "Database update failed. Contact the administrator" });
            }

        }
        
        // DELETE: Delete Student [NO BLL] [Return STATUS] [SuperAdmin]
        [HttpDelete("delete")]
        [Authorize(Policy = "SuperAdmin")]
        public async Task<ActionResult> Delete(int id)
        {
            // Find existing Cohort record in DB
            Student student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                Logger.Msg<StudentsController>($"[{User.Claims.Where(x => x.Type == "email").FirstOrDefault().Value}] [DELETE] StudentID: {id} not found", Serilog.Events.LogEventLevel.Debug);
                return NotFound();
            }

            try
            {
                // This will fail if the student has other records which would be orphaned
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();

                Logger.Msg<StudentsController>($"[{User.Claims.Where(x => x.Type == "email").FirstOrDefault().Value}] [DELETE] StudentID: {id} success", Serilog.Events.LogEventLevel.Information);
                return Ok(new StudentDTO(student));
            }
            catch (Exception ex)
            {
                // Probably due to FK violation
                Logger.Msg<StudentsController>($"[DELETE] Database sync error {ex.Message}", Serilog.Events.LogEventLevel.Error);

                // Return response to client
                return StatusCode(500, new { errors = "Database update failed. Perhaps there are students in this cohort?" });
            }

        }
    }
}
