using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
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

            // Due to the number of checks, this approach is more appropriate
            Dictionary<string, bool> exceptionTests = new Dictionary<string, bool>()
            {
                { "title cannot be empty.", task.Title == null || task.Title.Trim() == string.Empty },
                { "startDate cannot be in the past.", task.StartDate.Date < DateTime.Today },
                { "endDate cannot be in the past", task.EndDate.Date < DateTime.Today },
                { "Specified UnitID does not exist", !_context.Units.Any(x => x.UnitID == task.UnitID) },
                { "Specified CohortID does not exist", !_context.Cohorts.Any(x => x.CohortID == task.CohortID) },
                { "Specified TypeID does not exist", !_context.TaskTypes.Any(x => x.TypeID == task.TypeID ) },
                { "Document URL must start with http:// or https://", !string.IsNullOrEmpty(task.DocURL) && !(task.DocURL.ToLower().StartsWith("http://") | task.DocURL.ToLower().StartsWith("https://"))},
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

    }

    
}
