using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using TCStudentRecordManagement.Controllers.Exceptions;
using TCStudentRecordManagement.Models;
using TCStudentRecordManagement.Models.DTO;

namespace TCStudentRecordManagement.Controllers.BLL
{
    internal class TimesheetBLL
    {
        private readonly DataContext _context;

        internal TimesheetBLL(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Business logic for processing Timesheet record addition
        /// </summary>
        /// <param name="timesheetRecord"></param>
        /// <returns></returns>
        internal object AddTimesheetRecordBLL(TimesheetDTO timesheetRecord, ClaimsPrincipal userClaims)
        {
            // Check if the current user is a staff member (or Super Admin)
            bool isUserStaff = userClaims.IsInRole("Staff") || userClaims.IsInRole("SuperAdmin");

            // Get the StudentID of the currrent user (or 0)
            int currentStudentID = isUserStaff ? 0 : _context.Users.Where(x => x.Email == userClaims.FindFirstValue("email")).Include(users => users.StudentData).FirstOrDefault()?.StudentData.StudentID ?? 0;

            // Create a new APIException object to store possible exceptions as checks are performed. 
            APIException exceptionList = new APIException();

            Dictionary<string, bool> exceptionTests = new Dictionary<string, bool>()
            {
                { "Specified StudentID does not exist", !_context.Students.Any(x => x.StudentID == timesheetRecord.StudentID) },
                { "StudentID cannot differ from the authenticated user", !(timesheetRecord.StudentID == currentStudentID || isUserStaff)  },
                { "Student account matching the StudentID must be active", !(_context.Students.Where(x => x.StudentID == timesheetRecord.StudentID).Include(student => student.UserData).FirstOrDefault().UserData.Active || isUserStaff) },
                { "Specified AssignmentID does not exist", !_context.Tasks.Any(x => x.TaskID == timesheetRecord.AssignmentID) },
                { "Specified date is older than 7 days", !(((DateTime.Today - timesheetRecord.Date).Days > 7) || isUserStaff) },
                { "Specified date cannot be in the future", !(timesheetRecord.Date <= DateTime.Today) },
                { "TimeAllocation has to be a positive number, or zero", !(timesheetRecord.TimeAllocation >= 0) }
            };

            foreach (KeyValuePair<string, bool> kvp in exceptionTests)
            {
                if (kvp.Value) exceptionList.AddExMessage(kvp.Key);
            }

            if (!exceptionList.HasExceptions)
            {
                // Round the TimeAlllocation to 15 min (0.25h)
                // Ref: https://stackoverflow.com/a/2826278/12802214
                timesheetRecord.TimeAllocation = Math.Round(timesheetRecord.TimeAllocation * 4, MidpointRounding.ToEven) / 4;
                
                return timesheetRecord;
            }
            else
            {
                return exceptionList;
            }

        } // End of AddTimesheetRecordBLL
    

        /// <summary>
        /// Business logic for modifying an existing Timesheet record
        /// </summary>
        /// <param name="timesheetRecord"></param>
        /// <returns></returns>
        internal object ModifyTimesheetRecordBLL(TimesheetDTO timesheetModRecord, ClaimsPrincipal userClaims)
        {
            // Check if the current user is a staff member (or Super Admin)
            bool isUserStaff = userClaims.IsInRole("Staff") || userClaims.IsInRole("SuperAdmin");

            // Get the StudentID of the currrent user (or 0)
            int currentStudentID = isUserStaff ? 0 : _context.Users.Where(x => x.Email == userClaims.FindFirstValue("email")).Include(users => users.StudentData).FirstOrDefault()?.StudentData.StudentID ?? 0;

            // Create a new APIException object to store possible exceptions as checks are performed. 
            APIException exceptionList = new APIException();

            Dictionary<string, bool> exceptionTests = new Dictionary<string, bool>()
            {
                { "Specified StudentID does not exist", !_context.Students.Any(x => x.StudentID == timesheetModRecord.StudentID) },
                { "StudentID cannot differ from the authenticated user", !(timesheetModRecord.StudentID == currentStudentID || isUserStaff)  },
                { "StudentID in the record cannot differ from the StudentID of the authenticated user", !(_context.Timesheets.Where(x => x.RecordID == timesheetModRecord.RecordID).First().StudentID == currentStudentID) },
                { "Specified AssignmentID does not exist", !_context.Tasks.Any(x => x.TaskID == timesheetModRecord.AssignmentID) },
                { "Specified date is older than 7 days", !(((DateTime.Today - timesheetModRecord.Date).Days > 7) || isUserStaff) },
                { "Specified date cannot be in the future", !(timesheetModRecord.Date <= DateTime.Today) },
                { "TimeAllocation has to be a positive number, or zero", !(timesheetModRecord.TimeAllocation >= 0) },
                { "TimeAllocation has to be less than or equal to 18h", !(timesheetModRecord.TimeAllocation <= 18) }

            };

            foreach (KeyValuePair<string, bool> kvp in exceptionTests)
            {
                if (kvp.Value) exceptionList.AddExMessage(kvp.Key);
            }

            if (!exceptionList.HasExceptions)
            {
                // Round the TimeAlllocation to 15 min (0.25h)
                // Ref: https://stackoverflow.com/a/2826278/12802214
                timesheetModRecord.TimeAllocation = Math.Round(timesheetModRecord.TimeAllocation * 4, MidpointRounding.ToEven) / 4;

                return timesheetModRecord;
            }
            else
            {
                return exceptionList;
            }

        } // End of ModifyTimesheetRecordBLL
    
    }
}
