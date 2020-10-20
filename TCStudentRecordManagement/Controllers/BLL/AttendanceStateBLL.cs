using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCStudentRecordManagement.Controllers.Exceptions;
using TCStudentRecordManagement.Models;
using TCStudentRecordManagement.Models.DTO;

namespace TCStudentRecordManagement.Controllers.BLL
{
    internal class AttendanceStateBLL
    {
        private readonly DataContext _context;

        internal AttendanceStateBLL(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Business logic for processing AttendanceState record addition
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        internal object AddAttendanceStateBLL(string description)
        {
            // Create a new APIException object to store possible exceptions as checks are performed. 
            APIException exceptionList = new APIException();

            Dictionary<string, bool> exceptionTests = new Dictionary<string, bool>()
            {
                { "description parameter cannot be empty", description == null || description.Trim() == string.Empty },
                { "description must be unique", !string.IsNullOrEmpty(description) && _context.AttendanceStates.Any(x => x.Description.ToLower() == description.Trim().ToLower()) }
            };

            foreach (KeyValuePair<string, bool> kvp in exceptionTests)
            {
                if (kvp.Value) exceptionList.AddExMessage(kvp.Key);
            }

            if (!exceptionList.HasExceptions)
            {
                // Capitalise the first letter of the first word of the description
                string dbDescription = $"{description.Trim().Substring(0, 1).ToUpper()}{description.Trim().Substring(1)?.ToLower()}";

                AttendanceStateDTO newDbObject = new AttendanceStateDTO { Description = dbDescription };

                return newDbObject;
            }
            else
            {
                return exceptionList;
            }

        } // End of AddAttendanceStateBLL

        /// <summary>
        /// Business logic for processing AttendanceState description change. 
        /// </summary>
        /// <param name="taskType"></param>
        /// <returns></returns>
        internal object ModifyAttendanceStateBLL(AttendanceStateDTO attendanceState)
        {
            // Create a new APIException object to store possible exceptions as checks are performed. 
            APIException exceptionList = new APIException();

            // Due to the number of checks, this approach is more appropriate
            Dictionary<string, bool> exceptionTests = new Dictionary<string, bool>()
            {
                { "Specified StateID does not exist", !_context.AttendanceStates.Any(x => x.StateID == attendanceState.StateID) },
                { "description parameter cannot be empty", attendanceState.Description == null || attendanceState.Description.Trim() == string.Empty },
                { "description must be unique", !string.IsNullOrEmpty(attendanceState.Description) && _context.AttendanceStates.Any(x => x.Description.ToLower() == attendanceState.Description.Trim().ToLower() && x.StateID != attendanceState.StateID) }
            };

            foreach (KeyValuePair<string, bool> kvp in exceptionTests)
            {
                if (kvp.Value) exceptionList.AddExMessage(kvp.Key);
            }

            if (!exceptionList.HasExceptions)
            {
                // Capitalise the first letter of the first word of the description
                string dbDescription = $"{attendanceState.Description.Trim().Substring(0, 1).ToUpper()}{attendanceState.Description.Trim().Substring(1)?.ToLower()}";
                attendanceState.Description = dbDescription;
                return attendanceState;
            }
            else
            {
                return exceptionList;
            }
        } // End of ModifyAttendanceStateBLL

    }
}

