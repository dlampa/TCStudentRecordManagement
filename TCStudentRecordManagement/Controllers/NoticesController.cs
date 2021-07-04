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
using TCStudentRecordManagement.Models;
using TCStudentRecordManagement.Models.DTO;
using TCStudentRecordManagement.Utils;

namespace TCStudentRecordManagement.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class NoticesController : ControllerBase
    {
        private readonly DataContext _context;

        public NoticesController(DataContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Retrieve Notice records based on chosen criteria
        /// </summary>
        /// <param name="noticeID"></param>
        /// <param name="cohortID"></param>
        /// <param name="staffID"></param>
        /// <param name="validFrom"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NoticeDTO>>> Get(int noticeID, int cohortID, int staffID, DateTime validFrom)
        {

            // Check if the current user is a staff member (or Super Admin)
            bool isUserStaff = User.IsInRole("Staff") || User.IsInRole("SuperAdmin");

            // Call GetNoticekBLL method with all the parameters
            object BLLResponse = new NoticeBLL(_context).GetNoticeBLL(noticeID: noticeID, cohortID: cohortID, staffID: staffID, validFrom: validFrom, isUserStaff: isUserStaff);

            // Get the base class for the response
            // Ref: https://docs.microsoft.com/en-us/dotnet/api/system.type.basetype?view=netcore-3.1
            if (BLLResponse.GetType().BaseType == typeof(Exception))
            {
                // Create log entries for Debug log
                ((APIException)BLLResponse).Exceptions.ForEach(ex => Logger.Msg<NoticesController>((Exception)ex, Serilog.Events.LogEventLevel.Debug));

                // Return response from API
                return BadRequest(new { errors = ((APIException)BLLResponse).Exceptions.Select(x => x.Message).ToArray() });
            }
            else
            {
                try
                {
                    NoticeDTO BLLResponseDTO = (NoticeDTO)BLLResponse;

                    // Apply all the criteria with supplied or default values from BLL. Limit Student access to notices issued up to current date.
                    IQueryable<Notice> dbRequest = _context.Notices
                        .Where(x => x.ValidFrom >= BLLResponseDTO.ValidFrom && (isUserStaff || x.ValidFrom <= DateTime.Today));

                    if (BLLResponseDTO.CohortID != 0) dbRequest = dbRequest.Where(x => x.CohortID == BLLResponseDTO.CohortID);
                    if (BLLResponseDTO.StaffID != 0 && isUserStaff) dbRequest = dbRequest.Where(x => x.StaffID == BLLResponseDTO.StaffID);

                    List<Notice> dbResponse = await dbRequest.ToListAsync();

                    // Convert result to TaskDTO
                    List<NoticeDTO> response = new List<NoticeDTO>();
                    dbResponse.ForEach(x => response.Add(new NoticeDTO(x)));

                    Logger.Msg<NoticesController>($"[{User.FindFirstValue("email")}] [GET] ", Serilog.Events.LogEventLevel.Debug);
                    return Ok(response);
                }
                catch (Exception ex)
                {
                    // Local log entry. Database reconciliation issues are more serious so reported as Error
                    Logger.Msg<TasksController>($"[GET] Database sync error {ex.Message}", Serilog.Events.LogEventLevel.Error);

                    // Return response to client
                    return StatusCode(500, new { errors = "Database update failed. Contact the administrator to resolve this issue." });
                }

            }
        }// End of Get

        /// <summary>
        /// Add a Notice record to the database
        /// </summary>
        /// <param name="notice"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Policy = "StaffMember")]
        public async Task<ActionResult> AddNotice(NoticeDTO notice )
        {

            // Call BLL Notice Add method with all the parameters
            object BLLResponse = new NoticeBLL(_context).AddNoticeBLL(notice: notice, userClaims: User);

            // Get the base class for the response
            // Ref: https://docs.microsoft.com/en-us/dotnet/api/system.type.basetype?view=netcore-3.1
            if (BLLResponse.GetType().BaseType == typeof(Exception))
            {
                // Create log entries for Debug log
                ((APIException)BLLResponse).Exceptions.ForEach(ex => Logger.Msg<NoticesController>((Exception)ex, Serilog.Events.LogEventLevel.Debug));

                // Return response from API
                return BadRequest(new { errors = ((APIException)BLLResponse).Exceptions.Select(x => x.Message).ToArray() });
            }
            else
            {
                try
                {
                    // Convert BLLResponse to NoticeDTO object
                    NoticeDTO newNoticeDTO = (NoticeDTO)BLLResponse;

                    // Create a new Notice object
                    Notice newNotice = new Notice
                    {
                        HTML = newNoticeDTO.HTML,
                        Markdown = newNoticeDTO.Markdown,
                        CohortID = newNoticeDTO.CohortID,
                        StaffID = newNoticeDTO.StaffID,
                        ValidFrom = newNoticeDTO.ValidFrom
                    };

                    // Create the record
                    _context.Notices.Add(newNotice);
                    await _context.SaveChangesAsync();

                    Logger.Msg<NoticesController>($"[{User.FindFirstValue("email")}] [ADD] Notice '{newNotice.NoticeID}' successful", Serilog.Events.LogEventLevel.Information);

                    // Convert back to DTO and return to user
                    NoticeDTO response = new NoticeDTO(newNotice);
                    return Ok(response);
                }
                catch (Exception ex)
                {
                    // Local log entry. Database reconciliation issues are more serious so reported as Error
                    Logger.Msg<NoticesController>($"[ADD] Database sync error {ex.Message}", Serilog.Events.LogEventLevel.Error);

                    // Return response to client
                    return StatusCode(500, new { errors = "Database update failed. Contact the administrator to resolve this issue." });
                }
            }

        } // End of AddNotice

        /// <summary>
        /// Modify an existing Notice record
        /// </summary>
        /// <param name="modNotice"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Policy = "StaffMember")]
        public async Task<ActionResult> ModifyNotice([FromBody] NoticeDTO notice)
        {
            if (NoticeExists(notice.NoticeID))
            {

                // Call BLL Notice Modify method with all the parameters
                object BLLResponse = new NoticeBLL(_context).ModifyNoticeBLL(notice: notice, userClaims: User);

                if (BLLResponse.GetType().BaseType == typeof(Exception))
                {
                    // Create log entries for Debug log
                    ((APIException)BLLResponse).Exceptions.ForEach(ex => Logger.Msg<NoticesController>((Exception)ex, Serilog.Events.LogEventLevel.Debug));

                    // Return response from API
                    return BadRequest(new { errors = ((APIException)BLLResponse).Exceptions.Select(x => x.Message).ToArray() });
                }
                else
                {
                    try
                    {
                        NoticeDTO modNotice = (NoticeDTO)BLLResponse;

                        // Find the existing record based on ID
                        Notice currentRecord = _context.Notices.Where(x => x.NoticeID == modNotice.NoticeID).First();

                        // Modify the record
                        currentRecord.HTML = modNotice.HTML;
                        currentRecord.Markdown = modNotice.Markdown;
                        currentRecord.ValidFrom = modNotice.ValidFrom;
                        currentRecord.StaffID = modNotice.StaffID;

                        // Save changes
                        await _context.SaveChangesAsync();

                        Logger.Msg<NoticesController>($"[{User.FindFirstValue("email")}] [MODIFY] NoticeID: {currentRecord.NoticeID} successful", Serilog.Events.LogEventLevel.Information);

                        // Return modified record as a DTO
                        NoticeDTO response = new NoticeDTO(currentRecord);
                        return Ok(response);
                    }
                    catch (Exception ex)
                    {
                        // Local log entry. Database reconciliation issues are more serious so reported as Error
                        Logger.Msg<TaskTypesController>($"[MODIFY] Database sync error {ex.Message}", Serilog.Events.LogEventLevel.Error);

                        // Return response to client
                        return StatusCode(500, new { errors = "Database update failed. Contact the administrator to resolve this issue." });
                    }
                }
            }
            else
            {
                return NotFound();
            }
        } // End of ModifyNotice
        
        /// <summary>
        /// Deletes the Notice record
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(Policy = "StaffMember")]
        public async Task<ActionResult> Delete(int id)
        {
            // Find existing TaskType record in DB
            Notice notice = await _context.Notices.FindAsync(id);

            if (notice == null)
            {
                Logger.Msg<NoticesController>($"[{User.FindFirstValue("email")}] [DELETE] NoticeID: {id} not found", Serilog.Events.LogEventLevel.Debug);
                return NotFound();
            }

            try
            {
                _context.Notices.Remove(notice);
                await _context.SaveChangesAsync();

                Logger.Msg<NoticesController>($"[{User.FindFirstValue("email")}] [DELETE] NoticeID: {id} success", Serilog.Events.LogEventLevel.Information);
                return Ok(new NoticeDTO(notice));
            }
            catch (Exception ex)
            {
                Logger.Msg<NoticesController>($"[DELETE] Database sync error {ex.Message}", Serilog.Events.LogEventLevel.Error);

                // Return response to client
                return StatusCode(500, new { errors = "Database update failed." });
            }

        } // End of Delete

        /// <summary>
        /// Checks for existence of Notice record in the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool NoticeExists(int id)
        {
            return _context.Notices.Any(e => e.NoticeID == id);
        }
    }
}
