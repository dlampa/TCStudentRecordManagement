using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TCStudentRecordManagement.Controllers.Exceptions;
using TCStudentRecordManagement.Models;
using TCStudentRecordManagement.Models.DTO;

namespace TCStudentRecordManagement.Controllers.BLL
{
    internal class TaskBLL
    {
        private readonly DataContext _context;

        internal TaskBLL(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Business logic for processing Task record addition
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        internal object AddTaskBLL(TaskDTO task)
        {
            // Create a new APIException object to store possible exceptions as checks are performed. 
            APIException exceptionList = new APIException();

            // URI check based on: https://stackoverflow.com/a/37051246/12802214
            Dictionary<string, bool> exceptionTests = new Dictionary<string, bool>()
            {
                { "title cannot be empty.", task.Title == null || task.Title.Trim() == string.Empty },
                { "startDate cannot be in the past.", task.StartDate.Date < DateTime.Today },
                { "endDate cannot be in the past", task.EndDate.Date < DateTime.Today },
                { "Specified UnitID does not exist", !_context.Units.Any(x => x.UnitID == task.UnitID) },
                { "Specified CohortID does not exist", !_context.Cohorts.Any(x => x.CohortID == task.CohortID) },
                { "Specified TypeID does not exist", !_context.TaskTypes.Any(x => x.TypeID == task.TypeID ) },
                { "Document URL must start with http:// or https://", !string.IsNullOrEmpty(task.DocURL) && !(task.DocURL.ToLower().StartsWith("http://") | task.DocURL.ToLower().StartsWith("https://"))},
                { "Document URL must be valid", !string.IsNullOrEmpty(task.DocURL) && !Uri.IsWellFormedUriString(task.DocURL, UriKind.Absolute) },
                { "Specified cohort is inactive", !_context.Cohorts.Any(x => x.CohortID == task.CohortID && DateTime.Now >= x.StartDate && DateTime.Now <= x.EndDate) }
            };

            foreach (KeyValuePair<string, bool> kvp in exceptionTests)
            {
                if (kvp.Value) exceptionList.AddExMessage(kvp.Key);
            }

            if (!exceptionList.HasExceptions)
            {
                return task;
            }
            else
            {
                return exceptionList;
            }

        } // End of AddTaskBLL

        /// <summary>
        /// Business logic for processing modifications to the Task record
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        internal object ModifyTaskBLL(TaskDTO task)
        {
            // Create a new APIException object to store possible exceptions as checks are performed. 
            APIException exceptionList = new APIException();

            Dictionary<string, bool> exceptionTests = new Dictionary<string, bool>()
            {
                { "title cannot be empty.", task.Title == null || task.Title.Trim() == string.Empty },
                { "Specified UnitID does not exist", !_context.Units.Any(x => x.UnitID == task.UnitID) },
                { "Specified CohortID does not exist", !_context.Cohorts.Any(x => x.CohortID == task.CohortID) },
                { "Specified TypeID does not exist", !_context.TaskTypes.Any(x => x.TypeID == task.TypeID ) },
                { "Document URL must start with http:// or https://", !string.IsNullOrEmpty(task.DocURL) && !(task.DocURL.ToLower().StartsWith("http://") | task.DocURL.ToLower().StartsWith("https://"))},
                { "Document URL must be valid", !string.IsNullOrEmpty(task.DocURL) && !Uri.IsWellFormedUriString(task.DocURL, UriKind.Absolute) },
                { "Specified cohort is inactive.", !_context.Cohorts.Any(x => x.CohortID == task.CohortID && DateTime.Now >= x.StartDate && DateTime.Now <= x.EndDate) }
            };

            foreach (KeyValuePair<string, bool> kvp in exceptionTests)
            {
                if (kvp.Value) exceptionList.AddExMessage(kvp.Key);
            }

            if (!exceptionList.HasExceptions)
            {
                return task;
            }
            else
            {
                return exceptionList;
            }

        } // End of ModifyTaskBLL

        /// <summary>
        /// Business logic for retrieving Task records
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="cohortID"></param>
        /// <param name="typeID"></param>
        /// <param name="unitID"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="userClaims"></param>
        /// <returns></returns>
        internal object GetTaskBLL(int taskID, int cohortID, int typeID, int unitID, DateTime startDate, DateTime endDate, ClaimsPrincipal userClaims)
        {
            // Check if the current user is a staff member (or Super Admin)
            bool isUserStaff = userClaims.IsInRole("Staff") || userClaims.IsInRole("SuperAdmin");

            // Get the CohortID of the currrent user (or null)
            int currentCohortID = isUserStaff ? 0 : _context.Users.Where(x => x.Email == userClaims.FindFirstValue("email")).Include(users => users.StudentData).FirstOrDefault()?.StudentData.CohortID ?? 0;

            // Apply defaults
            if (startDate == DateTime.MinValue) startDate = DateTime.Today.AddDays(-14);
            if (endDate == DateTime.MinValue) endDate = DateTime.Today.AddDays(7);

            // Do not allow students to see future assignments
            if (startDate > DateTime.Today && !isUserStaff) startDate = DateTime.Today;
            if (cohortID == 0) cohortID = currentCohortID;

            // Create a new APIException object to store possible exceptions as checks are performed. 
            APIException exceptionList = new APIException();

            // Due to the number of checks, this approach is more appropriate
            Dictionary<string, bool> exceptionTests = new Dictionary<string, bool>()
            {
                { "cohortID has to match the current user's CohortID", !(cohortID == currentCohortID || isUserStaff) },
                { "taskID must be valid.", taskID != 0 && !_context.Tasks.Any(x => x.TaskID == taskID)},
                { "cohortID must be valid.", cohortID != 0 && !_context.Cohorts.Any(x => x.CohortID == cohortID)},
                { "typeID must be valid.", typeID != 0 && !_context.TaskTypes.Any(x => x.TypeID == typeID)},
                { "unitID must be valid.", unitID != 0 && !_context.Units.Any(x => x.UnitID == unitID)},
                { "Start date cannot be after the end date", !(startDate <= endDate) }
            };

            foreach (KeyValuePair<string, bool> kvp in exceptionTests)
            {
                if (kvp.Value) exceptionList.AddExMessage(kvp.Key);
            }

            if (!exceptionList.HasExceptions)
            {
                TaskDTO getParams = new TaskDTO
                {
                    TaskID = taskID,
                    CohortID = cohortID,
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

        }

    }


}
