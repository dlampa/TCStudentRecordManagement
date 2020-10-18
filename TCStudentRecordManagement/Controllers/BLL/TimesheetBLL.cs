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
        /// Business logic for retrieving Timesheet records
        /// </summary>
        /// <param name="recordID"></param>
        /// <param name="studentID"></param>
        /// <param name="assignmentID"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="userClaims"></param>
        /// <returns></returns>
        internal object GetTimesheetRecordBLL(int recordID, int studentID, int unitID, int typeID, int assignmentID, int cohortID, DateTime startDate, DateTime endDate, ClaimsPrincipal userClaims)
        {
            // Check if the current user is a staff member (or Super Admin)
            bool isUserStaff = userClaims.IsInRole("Staff") || userClaims.IsInRole("SuperAdmin");

            // Get the StudentID of the currrent user (or null)
            int currentStudentID = isUserStaff ? 0 : _context.Users.Where(x => x.Email == userClaims.FindFirstValue("email")).Include(users => users.StudentData).FirstOrDefault()?.StudentData.StudentID ?? 0;

            // Apply defaults
            if (startDate == DateTime.MinValue) startDate = DateTime.Today;
            if (endDate == DateTime.MinValue) endDate = DateTime.Today;
            if (!isUserStaff) studentID = currentStudentID;

            // Create a new APIException object to store possible exceptions as checks are performed. 
            APIException exceptionList = new APIException();

            Dictionary<string, bool> exceptionTests = new Dictionary<string, bool>()
            {
                { "recordID must be valid", recordID != 0 && !_context.Timesheets.Any(x => x.RecordID == recordID) },
                { "cohortID must be valid", cohortID != 0 && !_context.Cohorts.Any(x => x.CohortID == cohortID)},
                { "unitID must be valid", unitID != 0 && !_context.Units.Any(x => x.UnitID == unitID)},
                { "typeID must be valid", typeID != 0 && !_context.TaskTypes.Any(x => x.TypeID == typeID)},
                { "assignmentID must be valid", assignmentID != 0 && !_context.Tasks.Any(x => x.TaskID == assignmentID)},
                { "studentID must be valid", studentID != 0 && !_context.Students.Any(x => x.StudentID == studentID)},
                { "startDate cannot be after the endDate", !(startDate <= endDate) }
            };

            foreach (KeyValuePair<string, bool> kvp in exceptionTests)
            {
                if (kvp.Value) exceptionList.AddExMessage(kvp.Key);
            }

            if (!exceptionList.HasExceptions)
            {
                TimesheetGetDTO getParams = new TimesheetGetDTO
                {
                    RecordID = recordID,
                    StudentID = studentID,
                    CohortID = cohortID,
                    AssignmentID = assignmentID,
                    TypeID = typeID,
                    UnitID = unitID,
                    StartDate = startDate,
                    EndDate = endDate
                };

                return getParams;
            }
            else
            {
                return exceptionList;
            }

        } // End of GetTimesheetRecordBLL

        /// <summary>
        /// Business logic for processing Timesheet record addition
        /// </summary>
        /// <param name="timesheetRecord"></param>
        /// <param name="userClaims"></param>
        /// <returns></returns>
        internal object AddTimesheetRecordBLL(TimesheetDTO timesheetRecord, ClaimsPrincipal userClaims)
        {

            // Check if the current user is a staff member (or Super Admin)
            bool isUserStaff = userClaims.IsInRole("Staff") || userClaims.IsInRole("SuperAdmin");

            // Get the StudentID of the currrent user (or 0)
            int currentStudentID = isUserStaff ? 0 : _context.Users.Where(x => x.Email == userClaims.FindFirstValue("email")).Include(users => users.StudentData).FirstOrDefault()?.StudentData.StudentID ?? 0;

            if (timesheetRecord.StudentID == 0 && !isUserStaff) timesheetRecord.StudentID = currentStudentID;

            // Create a new APIException object to store possible exceptions as checks are performed. 
            APIException exceptionList = new APIException();

            Dictionary<string, bool> exceptionTests = new Dictionary<string, bool>()
            {
                { "Specified StudentID does not exist", !_context.Students.Any(x => x.StudentID == timesheetRecord.StudentID) },
                { "StudentID cannot differ from the authenticated user", !(timesheetRecord.StudentID == currentStudentID || isUserStaff)  },
                { "Student account matching the StudentID must be active", !(_context.Students.Where(x => x.StudentID == timesheetRecord.StudentID).Include(student => student.UserData).FirstOrDefault().UserData.Active || isUserStaff) },
                { "Specified AssignmentID does not exist", !_context.Tasks.Any(x => x.TaskID == timesheetRecord.AssignmentID) },
                { "Specified date is older than 7 days", ((DateTime.Today - timesheetRecord.Date).Days > 7) && !isUserStaff },
                { "Specified date cannot be in the future", !(timesheetRecord.Date <= DateTime.Today) },
                { "TimeAllocation has to be a positive number, or zero", !(timesheetRecord.TimeAllocation >= 0) },
                { "TimeAllocation has to be less than or equal to 18h", !(timesheetRecord.TimeAllocation <= 18) },
                { "Only one record per AssignmentID per date is allowed", _context.Timesheets.Any(x => x.Date == timesheetRecord.Date && x.AssignmentID == timesheetRecord.AssignmentID && x.StudentID == timesheetRecord.StudentID) }
            };

            foreach (KeyValuePair<string, bool> kvp in exceptionTests)
            {
                if (kvp.Value) exceptionList.AddExMessage(kvp.Key);
            }

            if (!exceptionList.HasExceptions)
            {
                // Round the TimeAlllocation to 15 min (0.25h)
                // Ref: https://stackoverflow.com/a/2826278/12802214
                timesheetRecord.TimeAllocation = Math.Round(timesheetRecord.TimeAllocation * 4, MidpointRounding.AwayFromZero) / 4;
                
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
        /// <param name="userClaims"></param>
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
                { "StudentID in the record cannot differ from the StudentID of the authenticated user", !(_context.Timesheets.Where(x => x.RecordID == timesheetModRecord.RecordID).First().StudentID == currentStudentID || isUserStaff) },
                { "Specified AssignmentID does not exist", !_context.Tasks.Any(x => x.TaskID == timesheetModRecord.AssignmentID) },
                { "Specified date is older than 7 days", !(((DateTime.Today - _context.Timesheets.Where(x => x.RecordID == timesheetModRecord.RecordID).First().Date).Days > 7) || isUserStaff) },
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
                timesheetModRecord.TimeAllocation = Math.Round(timesheetModRecord.TimeAllocation * 4, MidpointRounding.AwayFromZero) / 4;

                return timesheetModRecord;
            }
            else
            {
                return exceptionList;
            }

        } // End of ModifyTimesheetRecordBLL
    
        /// <summary>
        /// Business Logic for deleting an existing record
        /// </summary>
        /// <param name="recordID"></param>
        /// <param name="userClaims"></param>
        /// <returns></returns>
        internal object DeleteTimesheetRecordBLL(int recordID, ClaimsPrincipal userClaims)
        {

            // Check if the current user is a staff member (or Super Admin)
            bool isUserStaff = userClaims.IsInRole("Staff") || userClaims.IsInRole("SuperAdmin");

            // Get the StudentID of the currrent user (or 0)
            int currentStudentID = isUserStaff ? 0 : _context.Users.Where(x => x.Email == userClaims.FindFirstValue("email")).Include(users => users.StudentData).FirstOrDefault()?.StudentData.StudentID ?? 0;

            // Create a new APIException object to store possible exceptions as checks are performed. 
            APIException exceptionList = new APIException();

            Dictionary<string, bool> exceptionTests = new Dictionary<string, bool>()
            {
                { "StudentID in the record cannot differ from the StudentID of the authenticated user", !(_context.Timesheets.Where(x => x.RecordID == recordID).First().StudentID == currentStudentID) || isUserStaff }
            };

            foreach (KeyValuePair<string, bool> kvp in exceptionTests)
            {
                if (kvp.Value) exceptionList.AddExMessage(kvp.Key);
            }

            if (!exceptionList.HasExceptions)
            {
                return recordID;
            }
            else
            {
                return exceptionList;
            }

        } // End of TimesheetDeleteBLL



    }
}
