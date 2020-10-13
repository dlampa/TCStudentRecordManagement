using System;
using System.Linq;
using TCStudentRecordManagement.Models;
using TCStudentRecordManagement.Controllers.Exceptions;

namespace TCStudentRecordManagement.Controllers.BLL
{

    public class UserBLL
    {
        private readonly DataContext _context;

        public UserBLL(DataContext context)
        {
            _context = context;
        }

        public object AddUser(string name, DateTime startDate, DateTime endDate)
        {
            // Create a new APIException object to store possible exceptions as checks are performed. 
            APIException exceptionList = new APIException();

            // Check if the name has been supplied or if it's empty after trimming
            if (name == null || name.Trim() == string.Empty)
            {
                exceptionList.AddExMessage("Name parameter cannot be empty.");
            }
            else
            {
                // Check if a cohort with the same name exists in the database (trim + case insensitive)
                bool dbContainsName = _context.Cohorts.Where(x => x.Name.ToLower() == name.Trim().ToLower()).Count() > 0;

                if (dbContainsName) exceptionList.AddExMessage("A cohort with the same name already exists in the database.");

                // Check that the startDate is less than endDate
                bool datesConsistent = startDate < endDate;
                if (!datesConsistent) exceptionList.AddExMessage("Start date must be before the end date, check for consistency.");

            }


            if (!exceptionList.HasExceptions)
            {
                // New database name will be trimmed and in Proper Case.
                string dbName = String.Join(" ", name.Trim().Split(" ").Select(x => $"{x.Substring(0, 1).ToUpper()}{x.Substring(1)?.ToLower()}").ToArray());

                Cohort newDbObject = new Cohort { Name = dbName, StartDate = startDate, EndDate = endDate };

                return newDbObject;
            }
            else
            {
                return exceptionList;
            }

        } // End of AddCohort
    }
}
