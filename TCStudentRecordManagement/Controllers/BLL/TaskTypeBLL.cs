using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using TCStudentRecordManagement.Controllers.Exceptions;
using TCStudentRecordManagement.Models;
using TCStudentRecordManagement.Models.DTO;

namespace TCStudentRecordManagement.Controllers.BLL
{
    internal class TaskTypeBLL
    {
        private readonly DataContext _context;

        public TaskTypeBLL(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Business logic for processing TaskType record addition
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        internal object AddTaskTypeBLL(string description)
        {
            // Create a new APIException object to store possible exceptions as checks are performed. 
            APIException exceptionList = new APIException();

            Dictionary<string, bool> exceptionTests = new Dictionary<string, bool>()
            {
                { "description parameter cannot be empty", description == null || description.Trim() == string.Empty },
                { "description must be unique", _context.TaskTypes.Any(x => x.Description.ToLower() == description.Trim().ToLower()) }
            };

            foreach (KeyValuePair<string, bool> kvp in exceptionTests)
            {
                if (kvp.Value) exceptionList.AddExMessage(kvp.Key);
            }

            if (!exceptionList.HasExceptions)
            {
                // Capitalise the first letter of the first word of the description
                string dbDescription = $"{description.Trim().Substring(0, 1).ToUpper()}{description.Trim().Substring(1)?.ToLower()}";

                TaskTypeDTO newDbObject = new TaskTypeDTO { Description = dbDescription };

                return newDbObject;
            }
            else
            {
                return exceptionList;
            }

        } // End of AddTaskTypeBLL


    }
}
