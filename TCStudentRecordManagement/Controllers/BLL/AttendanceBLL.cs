using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Serilog.Sinks.SystemConsole.Themes;
using TCStudentRecordManagement.Controllers.Exceptions;
using TCStudentRecordManagement.Models;
using TCStudentRecordManagement.Models.DTO;

namespace TCStudentRecordManagement.Controllers.BLL
{
    internal class AttendanceBLL
    {
        private readonly DataContext _context;

        internal AttendanceBLL(DataContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Business logic for adding an attendance record to the database
        /// </summary>
        /// <param name="attendance"></param>
        /// <param name="userClaims"></param>
        /// <returns>object Exception() or object attendanceDTO</returns>
        internal object AddAttendanceBLL(AttendanceDTO attendance, ClaimsPrincipal userClaims)
        {
            // Get the StaffID of the currently logged in Staff member
            int loggedInStaffID = _context.Users.Where(x => x.Email == userClaims.FindFirst("Email").Value).Include(user => user.StaffData).Select(x => x.StaffData.StaffID).First();
            int studentUserID = _context.Students.Where(x => x.StudentID == attendance.StudentID).Select(x => x.UserID).First();

            // Is the user Super Admin?
            bool userIsSuperAdmin = userClaims.IsInRole("SuperAdmin");

            // Create a new APIException object to store possible exceptions as checks are performed. 
            APIException exceptionList = new APIException();

            // Due to the number of checks, this approach is more appropriate
            Dictionary<string, bool> exceptionTests = new Dictionary<string, bool>()
            {
                { "Specified StudentID does not exist", !_context.Students.Any(x => x.StudentID == attendance.StudentID) },
                { "Specified AttendanceStateID does not exist", !_context.AttendanceStates.Any(x => x.StateID == attendance.AttendanceStateID) },
                { "Specified StaffID does not exist", !_context.Staff.Any(x => x.StaffID == attendance.StaffID) },
                { "Specified StaffID does not match current user credentials", !(_context.Staff.Any(x => x.StaffID == attendance.StaffID) && loggedInStaffID == attendance.StaffID) },
                { "Duplicate record exists in the database (StudentID, Date)", _context.AttendanceRecords.Any(x => x.StudentID == attendance.StudentID && x.Date == attendance.Date) },
                { "Specified attendance record cannot be added as the specified date is more than 3 days old", !(((DateTime.Today - attendance.Date.Date).Days <= 3) || userIsSuperAdmin) },
                { "Specified student is inactive.", !(_context.Students.Where(x => x.StudentID == attendance.StudentID).Include(student => student.UserData).Select(x => x.UserData.Active).First() || userIsSuperAdmin) }
            };

            foreach (KeyValuePair<string, bool> kvp in exceptionTests)
            {
                if (kvp.Value) exceptionList.AddExMessage(kvp.Key);
            }

            if (!exceptionList.HasExceptions)
            {
                return attendance;
            }
            else
            {
                return exceptionList;
            }

        } // End of AddAttendanceBLL

        /// <summary>
        /// Business Logic for modifying Attendances table records
        /// </summary>
        /// <param name="attendance">AttendanceModDTO object with properties to modify</param>
        /// <param name="userClaims">ClaimsPrincipal object containing User Identity</param>
        /// <returns>object Exception or AttendanceModDTO</returns>
        internal object ModifyAttendanceBLL(AttendanceModDTO attendance, ClaimsPrincipal userClaims)
        {

            // Create a new APIException object to store possible exceptions as checks are performed. 
            APIException exceptionList = new APIException();

            // Due to the number of checks, this approach is more appropriate
            Dictionary<string, bool> exceptionTests = new Dictionary<string, bool>()
            {
                { "Specified RecordID does not exist", !_context.AttendanceRecords.Any(x => x.RecordID == attendance.RecordID) },
                { "Specified AttendanceStateID does not exist", !_context.AttendanceStates.Any(x => x.StateID == attendance.AttendanceStateID) }
            };

            foreach (KeyValuePair<string, bool> kvp in exceptionTests)
            {
                if (kvp.Value) exceptionList.AddExMessage(kvp.Key);
            }

            if (!exceptionList.HasExceptions)
            {
                return attendance;
            }
            else
            {
                return exceptionList;
            }
        } // End of ModifyAttendanceBLL

        /// <summary>
        /// Business Logic for processing Get requests for records in the attendance table
        /// </summary>
        /// <param name="studentID"></param>
        /// <param name="attendanceStateID"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="userClaims"></param>
        /// <returns></returns>
        internal object GetAttendanceBLL(int studentID, int attendanceStateID, DateTime startDate, DateTime endDate, ClaimsPrincipal userClaims)
        {
            // Check if the current user is a staff member (or Super Admin)
            bool isUserStaff = userClaims.IsInRole("Staff") || userClaims.IsInRole("SuperAdmin");

            // Get the StudentID of the currrent user (or null)
            int currentStudentID = isUserStaff? 0 : _context.Users.Where(x => x.Email == userClaims.FindFirstValue("email")).Include(users => users.StudentData).FirstOrDefault()?.StudentData.StudentID ?? 0;

            // Apply defaults
            if (startDate == DateTime.MinValue) startDate = DateTime.Today.AddDays(-7);
            if (endDate == DateTime.MinValue) endDate = DateTime.Today;
            if (studentID == 0) studentID = currentStudentID;

            // Create a new APIException object to store possible exceptions as checks are performed. 
            APIException exceptionList = new APIException();

            // Due to the number of checks, this approach is more appropriate
            Dictionary<string, bool> exceptionTests = new Dictionary<string, bool>()
            {
                { "StudentID has to match the current user StudentID", !(studentID == currentStudentID || isUserStaff) },
                { "Start date cannot be after the end date", !(startDate <= endDate) },
                { "Invalid attendanceStateID specified", !(_context.AttendanceStates.Any(x => x.StateID == attendanceStateID) || attendanceStateID == 0) }
            };

            foreach (KeyValuePair<string, bool> kvp in exceptionTests)
            {
                if (kvp.Value) exceptionList.AddExMessage(kvp.Key);
            }

            if (!exceptionList.HasExceptions)
            {

                // Create a dictionary containing all data needed to create the user and pass back to API Target
                Dictionary<string, object> getParams = new Dictionary<string, object>()
                {
                    { "StudentID", studentID },
                    { "AttendanceStateID", attendanceStateID},
                    { "StartDate", startDate},
                    { "EndDate", endDate}
                };

                return getParams;
            }
            else
            {
                return exceptionList;
            }

        }// End of GetAttendanceBLL
    }


}
