using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TCStudentRecordManagement.Models;
using TCStudentRecordManagement.Controllers.DTO;
using TCStudentRecordManagement.Utils;


namespace TCStudentRecordManagement.Controllers
{
    [Route("/users")]
    [Authorize(Policy = "SuperAdmin")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;

        public UsersController(DataContext context)
        {
            _context = context;
        }

        // GET: All Users [NO BLL] [Return DTO]
        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> List()
        {
            // Convert User to UserDTO
            List<User> userData = await _context.Users.ToListAsync();
            List<UserDTO> result = new List<UserDTO>();
            userData.ForEach(x => result.Add(new UserDTO(x)));

            // Log to debug log
            Logger.Msg<UsersController>($"[{User.Claims.Where(x => x.Type == "email").FirstOrDefault().Value}] [LIST]", Serilog.Events.LogEventLevel.Debug);
            return result;
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: Activate/Deactivate User [NO BLL] [Return DTO]
        [HttpPut("active")]
        public async Task<ActionResult> ChangeActiveState(int id, bool state)
        {
            User user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                Logger.Msg<UsersController>($"[{User.Claims.Where(x => x.Type == "email").FirstOrDefault().Value}] [ACTIVE] UserID: {id} not found", Serilog.Events.LogEventLevel.Debug);
                return NotFound();
            }

            try
            {
                // Change active bit to match parameter
                user.Active = state;

                // Save to DB
                await _context.SaveChangesAsync();

                Logger.Msg<UsersController>($"[{User.Claims.Where(x => x.Type == "email").FirstOrDefault().Value}] [ACTIVE] UserID: {id} {(state ? "activated" : "deactivated")}", Serilog.Events.LogEventLevel.Information);
                return Ok(new UserDTO(user));
            }
            catch (Exception ex)
            {
                Logger.Msg<UsersController>($"[ACTIVE] Database sync error {ex.Message}", Serilog.Events.LogEventLevel.Error);

                // Return response to client
                return StatusCode(500, new { errors = "Database update failed. Contact the administrator" });
            }

        }

        // DELETE: Delete user [NO BLL] [Return STATUS]
        [HttpDelete("delete")]
        public async Task<ActionResult> Delete(int id)
        {
            // Find existing Cohort record in DB
            User user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                Logger.Msg<UsersController>($"[{User.Claims.Where(x => x.Type == "email").FirstOrDefault().Value}] [DELETE] UserID: {id} not found", Serilog.Events.LogEventLevel.Debug);
                return NotFound();
            }

            try
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                Logger.Msg<UsersController>($"[{User.Claims.Where(x => x.Type == "email").FirstOrDefault().Value}] [DELETE] UserID: {id} success", Serilog.Events.LogEventLevel.Information);
                return Ok(new UserDTO(user));
            }
            catch (Exception ex)
            {
                // Probably due to FK violation
                Logger.Msg<UsersController>($"[DELETE] Database sync error {ex.Message}", Serilog.Events.LogEventLevel.Error);

                // Return response to client
                return StatusCode(500, new { errors = "Database update failed. Perhaps there are students in this cohort?" });
            }

        }





        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserID == id);
        }
    }
}
