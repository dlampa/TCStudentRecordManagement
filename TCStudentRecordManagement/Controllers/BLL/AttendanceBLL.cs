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
    public class AttendanceBLL
    {
        private readonly DataContext _context;

        public AttendanceBLL(DataContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Business logic for adding an attendance record to the database
        /// </summary>
        /// <param name="attendance"></param>
        /// <param name="userClaims"></param>
        /// <returns>object Exception() or object attendanceDTO</returns>
        public object AddAttendance(AttendanceDTO attendance, ClaimsPrincipal userClaims)
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
                { "Duplicate record exists in the database (StudentID, Date)", !_context.AttendanceRecords.Any(x => x.StudentID == attendance.StudentID && x.Date == attendance.Date) },
                { "Specified attendance record cannot be added as the specified date is more than 3 days old", !(((DateTime.Today - attendance.Date.Date).Days <= 3) || userIsSuperAdmin) },
                { "Specified student is inactive.", !((_context.Students.Where(x => x.StudentID ==  attendance.StudentID).Include(student => student.UserData).Select(x => x.UserData.Active).First()) || userIsSuperAdmin) }
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

        } // End of AddAttendance

        /// <summary>
        /// Business Logic for modifying Attendances table records
        /// </summary>
        /// <param name="attendance">AttendanceModDTO object with properties to modify</param>
        /// <param name="userClaims">ClaimsPrincipal object containing User Identity</param>
        /// <returns>object Exception or AttendanceModDTO</returns>
        internal object ModifyAttendance(AttendanceModDTO attendance, ClaimsPrincipal userClaims)
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
        }
    }

    
}
