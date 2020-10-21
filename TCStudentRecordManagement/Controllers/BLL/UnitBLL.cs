using System.Collections.Generic;
using System.Linq;
using TCStudentRecordManagement.Controllers.Exceptions;
using TCStudentRecordManagement.Models;
using TCStudentRecordManagement.Models.DTO;

namespace TCStudentRecordManagement.Controllers.BLL
{
    internal class UnitBLL
    {
        private readonly DataContext _context;

        internal UnitBLL(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Business logic for processing Unit record addition
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        internal object AddUnitBLL(string description)
        {
            // Create a new APIException object to store possible exceptions as checks are performed. 
            APIException exceptionList = new APIException();

            Dictionary<string, bool> exceptionTests = new Dictionary<string, bool>()
            {
                { "description parameter cannot be empty", description == null || description.Trim() == string.Empty },
                { "description must be unique", !string.IsNullOrEmpty(description) && _context.Units.Any(x => x.Description.ToLower() == description.Trim().ToLower()) }
            };

            foreach (KeyValuePair<string, bool> kvp in exceptionTests)
            {
                if (kvp.Value) exceptionList.AddExMessage(kvp.Key);
            }

            if (!exceptionList.HasExceptions)
            {
                // Capitalise the first letter of the first word of the description
                string dbDescription = $"{description.Trim().Substring(0, 1).ToUpper()}{description.Trim().Substring(1)?.ToLower()}";

                UnitDTO newDbObject = new UnitDTO { Description = dbDescription };

                return newDbObject;
            }
            else
            {
                return exceptionList;
            }

        } // End of AddTaskTypeBLL

        /// <summary>
        /// Business logic for processing Unit description change. 
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        internal object ModifyUnitBLL(UnitDTO unit)
        {
            // Create a new APIException object to store possible exceptions as checks are performed. 
            APIException exceptionList = new APIException();

            // Due to the number of checks, this approach is more appropriate
            Dictionary<string, bool> exceptionTests = new Dictionary<string, bool>()
            {
                { "Specified UnitID does not exist", !_context.Units.Any(x => x.UnitID == unit.UnitID) },
                { "description parameter cannot be empty", unit.Description == null || unit.Description.Trim() == string.Empty },
                { "description must be unique", !string.IsNullOrEmpty(unit.Description) && _context.Units.Any(x => x.Description.ToLower() == unit.Description.Trim().ToLower() && x.UnitID != unit.UnitID) }
            };

            foreach (KeyValuePair<string, bool> kvp in exceptionTests)
            {
                if (kvp.Value) exceptionList.AddExMessage(kvp.Key);
            }

            if (!exceptionList.HasExceptions)
            {
                // Capitalise the first letter of the first word of the description
                string dbDescription = $"{unit.Description.Trim().Substring(0, 1).ToUpper()}{unit.Description.Trim().Substring(1)?.ToLower()}";
                unit.Description = dbDescription;
                return unit;
            }
            else
            {
                return exceptionList;
            }
        } // End of ModifyUnitBLL
    }
}
