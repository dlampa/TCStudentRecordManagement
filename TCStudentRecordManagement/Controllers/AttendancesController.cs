using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    [Route("/attendance")]
    [Authorize(Policy = "StaffMember")]
    [ApiController]
    public class AttendancesController : ControllerBase
    {
        private readonly DataContext _context;

        public AttendancesController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Attendances
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Attendance>>> GetAttendanceRecords()
        {
            return await _context.AttendanceRecords.ToListAsync();
        }

        [HttpPut("add")]
        public async Task<ActionResult> AddAttendance([FromBody] AttendanceDTO attendance)
        {
            // Call BLL method to run validation
            object BLLResponse = new AttendanceBLL(_context).AddAttendance(attendance: attendance, userClaims: User);

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
                    Attendance newAttendanceRecord = new Attendance
                    {
                        StudentID = attendance.StudentID,
                        StaffID = attendance.StaffID,
                        AttendanceStateID = attendance.AttendanceStateID,
                        Date = attendance.Date,
                        Comment = attendance.Comment
                    };

                    _context.AttendanceRecords.Add(newAttendanceRecord);

                    // Save changes
                    await _context.SaveChangesAsync();

                    Logger.Msg<AttendancesController>($"[{User.Claims.Where(x => x.Type == "email").FirstOrDefault().Value}] [ADD] attendance for StudentID '{newAttendanceRecord.StudentID}' recorded", Serilog.Events.LogEventLevel.Information);
                    AttendanceDTO response = new AttendanceDTO(newAttendanceRecord);
                    return Ok(response);
                }
                catch (Exception ex)
                {
                    // Local log entry. Database reconciliation issues are more serious so reported as Error
                    Logger.Msg<AttendancesController>($"[MODIFY] Database sync error {ex.Message}", Serilog.Events.LogEventLevel.Error);

                    // Return response to client
                    return StatusCode(500, new { errors = "Database update failed. Contact the administrator to resolve this issue." });
                }
            }

        }

        /// <summary>
        /// Modify attendance records in the Attendances table
        /// </summary>
        /// <param name="attendance">AttendanceModDTO object containing parameters for modification</param>
        /// <returns>API HTTPResponse with embedded AttendanceModDTO object or Exception()[]</returns>
        [HttpPut("modify")]
        public async Task<ActionResult> ModifyAttendance([FromBody] AttendanceModDTO attendance)
        {
            
            if (AttendanceExists(attendance.RecordID))
            {

                // Call BLL method to run validation
                object BLLResponse = new AttendanceBLL(_context).ModifyAttendance(attendance: attendance, userClaims: User);

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
                        // Cast the response to the correct type
                        AttendanceModDTO modAttendanceRecord = (AttendanceModDTO)BLLResponse;

                        // Modify the Attendance Record
                        Attendance attendanceRecord = _context.AttendanceRecords.Where(x => x.RecordID == modAttendanceRecord.RecordID).First();

                        attendanceRecord.AttendanceStateID = attendance.AttendanceStateID;
                        attendanceRecord.Comment = attendance.Comment;
                     
                        // Save changes
                        await _context.SaveChangesAsync();

                        Logger.Msg<AttendancesController>($"[{User.Claims.Where(x => x.Type == "email").FirstOrDefault().Value}] [MODIFY] attendance RecorID '{attendanceRecord.RecordID}' updated", Serilog.Events.LogEventLevel.Information);
                        AttendanceDTO response = new AttendanceDTO(attendanceRecord);
                        return Ok(response);
                    }
                    catch (Exception ex)
                    {
                        // Local log entry. Database reconciliation issues are more serious so reported as Error
                        Logger.Msg<AttendancesController>($"[MODIFY] Database sync error {ex.Message}", Serilog.Events.LogEventLevel.Error);

                        // Return response to client
                        return StatusCode(500, new { errors = "Database update failed. Contact the administrator to resolve this issue." });
                    }
                }
            }
            else
            {
                return NotFound();
            }
            
        } // End of Modify
    


        // PUT: api/Attendances/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAttendance(int id, Attendance attendance)
        {
            if (id != attendance.RecordID)
            {
                return BadRequest();
            }

            _context.Entry(attendance).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AttendanceExists(id))
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

        // POST: api/Attendances
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Attendance>> PostAttendance(Attendance attendance)
        {
            _context.AttendanceRecords.Add(attendance);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAttendance", new { id = attendance.RecordID }, attendance);
        }

        // DELETE: api/Attendances/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Attendance>> DeleteAttendance(int id)
        {
            var attendance = await _context.AttendanceRecords.FindAsync(id);
            if (attendance == null)
            {
                return NotFound();
            }

            _context.AttendanceRecords.Remove(attendance);
            await _context.SaveChangesAsync();

            return attendance;
        }

        private bool AttendanceExists(int id)
        {
            return _context.AttendanceRecords.Any(e => e.RecordID == id);
        }
    }
}
