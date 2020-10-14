﻿using System;
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
    [Authorize]
    [ApiController]
    public class AttendancesController : ControllerBase
    {
        private readonly DataContext _context;

        public AttendancesController(DataContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Gets the Student attendance records based on StudentID. Students are limited to viewing their own records only.
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Attendance>>> Get_Target()
        {
            return await _context.AttendanceRecords.ToListAsync();
        }

        /// <summary>
        /// Add attendance record to the database.  Attendance record properties are read from the request body from a JSON representation of the AttendanceDTO object.
        /// </summary>
        /// <param name="attendance">AttendanceDTO object containing the new attendance record</param>
        /// <returns></returns>
        [HttpPut("add")]
        [Authorize(Policy = "StaffMember")]
        public async Task<ActionResult> AddAttendance_Target([FromBody] AttendanceDTO attendance)
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
        /// Modify attendance record in the Attendances table. Modified properties are read from the body of the request from a JSON representation of the AttendanceModDTO object.
        /// </summary>
        /// <param name="attendance">AttendanceModDTO object containing parameters for modification</param>
        /// <returns>API HTTPResponse with embedded AttendanceModDTO object or Exception()[]</returns>
        [HttpPut("modify")]
        [Authorize(Policy = "StaffMember")]
        public async Task<ActionResult> ModifyAttendance([FromBody] AttendanceModDTO attendance)
        {
            
            if (AttendanceExists(attendance.RecordID))
            {

                // Call BLL method to run validation
                object BLLResponse = new AttendanceBLL(_context).ModifyAttendanceBLL(attendance: attendance, userClaims: User);

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

        } // End of ModifyAttendance

        /// <summary>
        /// Delete an Attendance record based on RecordID. Available only to Super Admin users.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpDelete("delete")]
        [Authorize(Policy = "SuperAdmin")]
        public async Task<ActionResult> Delete(int id)
        {
            // Find existing Cohort record in DB
            Attendance attendance = await _context.AttendanceRecords.FindAsync(id);

            if (attendance == null)
            {
                Logger.Msg<AttendancesController>($"[{User.Claims.Where(x => x.Type == "email").FirstOrDefault().Value}] [DELETE] RecordID: {id} not found", Serilog.Events.LogEventLevel.Debug);
                return NotFound();
            }

            try
            {
                _context.AttendanceRecords.Remove(attendance);
                await _context.SaveChangesAsync();

                Logger.Msg<AttendancesController>($"[{User.Claims.Where(x => x.Type == "email").FirstOrDefault().Value}] [DELETE] RecordID: {id} success", Serilog.Events.LogEventLevel.Information);
                return Ok(new AttendanceDTO(attendance));
            }
            catch (Exception ex)
            {
                // Probably due to FK violation
                Logger.Msg<CohortsController>($"[DELETE] Database sync error {ex.Message}", Serilog.Events.LogEventLevel.Error);

                // Return response to client
                return StatusCode(500, new { errors = "Database update failed. Perhaps there are students in this cohort?" });
            }

        } // End of Delete

        private bool AttendanceExists(int id)
        {
            return _context.AttendanceRecords.Any(e => e.RecordID == id);
        }
    }
}
