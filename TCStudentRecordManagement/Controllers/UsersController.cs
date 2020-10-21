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
    [Authorize(Policy = "SuperAdmin")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;

        public UsersController(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all User records from the database and reports them as UserDTO object
        /// </summary>
        /// <returns></returns>
        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> List()
        {
            // Convert User to UserDTO
            List<User> userData = await _context.Users.ToListAsync();
            List<UserDTO> result = new List<UserDTO>();
            userData.ForEach(x => result.Add(new UserDTO(x)));

            // Log to debug log
            Logger.Msg<UsersController>($"[{User.FindFirstValue("email")}] [LIST]", Serilog.Events.LogEventLevel.Debug);
            if (result == null)
            {
                return NotFound();
            }
            else
            {
                return result;
            }

        } // End of List

        /// <summary>
        /// Retrieves user records from the database based on UserID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("get")]
        public async Task<ActionResult<UserDTO>> Get(int id)
        {
            // Convert User to UserDTO
            User userData = await _context.Users.FindAsync(id);
            UserDTO result = new UserDTO(userData);

            // Log to debug log
            Logger.Msg<UsersController>($"[{User.FindFirstValue("email")}] [GET]", Serilog.Events.LogEventLevel.Debug);

            if (result == null)
            {
                return NotFound();
            }
            else
            {
                return result;
            }

        } // End of Get

        /// <summary>
        /// Adds an user to the database.
        /// </summary>
        /// <param name="firstname"></param>
        /// <param name="lastname"></param>
        /// <param name="email"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        [HttpPut("add")]
        public async Task<ActionResult> AddUser(string firstname, string lastname, string email, bool active)
        {
            // Call BLL User Add method with all the parameters
            object BLLResponse = new UserBLL(_context).AddUserBLL(firstname: firstname, lastname: lastname, email: email, active: active);

            if (BLLResponse.GetType().BaseType == typeof(Exception))
            {
                // Create log entries for Debug log
                ((APIException)BLLResponse).Exceptions.ForEach(ex => Logger.Msg<UsersController>((Exception)ex, Serilog.Events.LogEventLevel.Debug));

                // Return response from API
                return BadRequest(new { errors = ((APIException)BLLResponse).Exceptions.Select(x => x.Message).ToArray() });
            }
            else
            {
                try
                {
                    UserDTO newUserDTO = (UserDTO)BLLResponse;

                    // Convert UserDTO to User
                    User newUser = new User
                    {
                        Firstname = newUserDTO.Firstname,
                        Lastname = newUserDTO.Lastname,
                        Email = newUserDTO.Email,
                        Active = newUserDTO.Active
                    };

                    // Add to model and save changes
                    _context.Users.Add(newUser);
                    await _context.SaveChangesAsync();

                    Logger.Msg<UsersController>($"[{User.FindFirstValue("email")}] [ADD] user '{newUser.Email}' successful", Serilog.Events.LogEventLevel.Information);
                    return Ok(newUserDTO);
                }
                catch (Exception ex)
                {
                    // Local log entry. Database reconciliation issues are more serious so reported as Error
                    Logger.Msg<UsersController>($"[ADD] Database sync error {ex.Message}", Serilog.Events.LogEventLevel.Error);

                    // Return response to client
                    return StatusCode(500, new { errors = "Database update failed. Contact the administrator to resolve this issue." });
                }
            }

        }// End of AddUser


        /// <summary>
        /// Modify user record in database.
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        [HttpPut("modify")]
        public async Task<ActionResult> ModifyUser(UserDTO user)
        {
            // Call BLL to validate inputs
            object BLLResponse = new UserBLL(_context).ModifyUserBLL(user: user);

            if (BLLResponse.GetType().BaseType == typeof(Exception))
            {
                // Create log entries for Debug log
                ((APIException)BLLResponse).Exceptions.ForEach(ex => Logger.Msg<UsersController>((Exception)ex, Serilog.Events.LogEventLevel.Debug));

                // Return response from API
                return BadRequest(new { errors = ((APIException)BLLResponse).Exceptions.Select(x => x.Message).ToArray() });
            }
            else
            {
                try
                {
                    UserDTO modifiedUserData = (UserDTO)BLLResponse;

                    // Modify User record accordingly

                    User userRecord = _context.Users.Where(x => x.UserID == modifiedUserData.UserID).First();

                    userRecord.Firstname = modifiedUserData.Firstname;
                    userRecord.Lastname = modifiedUserData.Lastname;
                    userRecord.Email = modifiedUserData.Email;
                    userRecord.Active = modifiedUserData.Active;

                    // Save changes
                    await _context.SaveChangesAsync();

                    Logger.Msg<UsersController>($"[{User.FindFirstValue("email")}] [MODIFY] user '{modifiedUserData.Email}' successful", Serilog.Events.LogEventLevel.Information);
                    UserDTO response = new UserDTO(userRecord);
                    return Ok(response);
                }
                catch (Exception ex)
                {
                    // Local log entry. Database reconciliation issues are more serious so reported as Error
                    Logger.Msg<UsersController>($"[MODIFY] Database sync error {ex.Message}", Serilog.Events.LogEventLevel.Error);

                    // Return response to client
                    return StatusCode(500, new { errors = "Database update failed. Contact the administrator to resolve this issue." });
                }
            }

        } // End of ModifyStudent_Target

        /// <summary>
        /// Activates or deactivates specific user's access to the system 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpPut("activate")]
        public async Task<ActionResult> ChangeActiveState(int id, bool state)
        {
            User user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                Logger.Msg<UsersController>($"[{User.FindFirstValue("email")}] [ACTIVE] UserID: {id} not found", Serilog.Events.LogEventLevel.Debug);
                return NotFound();
            }

            try
            {
                // Change active bit to match parameter
                user.Active = state;

                // Save to DB
                await _context.SaveChangesAsync();

                Logger.Msg<UsersController>($"[{User.FindFirstValue("email")}] [ACTIVE] UserID: {id} {(state ? "activated" : "deactivated")}", Serilog.Events.LogEventLevel.Information);
                return Ok(new UserDTO(user));
            }
            catch (Exception ex)
            {
                Logger.Msg<UsersController>($"[ACTIVE] Database sync error {ex.Message}", Serilog.Events.LogEventLevel.Error);

                // Return response to client
                return StatusCode(500, new { errors = "Database update failed. Contact the administrator" });
            }

        } // End of ChangeActiveState

        /// <summary>
        /// Deletes user record in the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("delete")]
        public async Task<ActionResult> Delete(int id)
        {
            // Find existing Cohort record in DB
            User user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                Logger.Msg<UsersController>($"[{User.FindFirstValue("email")}] [DELETE] UserID: {id} not found", Serilog.Events.LogEventLevel.Debug);
                return NotFound();
            }

            try
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                Logger.Msg<UsersController>($"[{User.FindFirstValue("email")}] [DELETE] UserID: {id} success", Serilog.Events.LogEventLevel.Information);
                return Ok(new UserDTO(user));
            }
            catch (Exception ex)
            {
                // Probably due to FK violation
                Logger.Msg<UsersController>($"[DELETE] Database sync error {ex.Message}", Serilog.Events.LogEventLevel.Error);

                // Return response to client
                return StatusCode(500, new { errors = "Database update failed." });
            }

        } // End of Delete

        /// <summary>
        /// Checks for existence of User records in the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserID == id);
        }
    }
}
