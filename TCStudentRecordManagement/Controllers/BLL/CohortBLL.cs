using System;
using System.Linq;
using TCStudentRecordManagement.Models;
using TCStudentRecordManagement.Controllers.Exceptions;
using TCStudentRecordManagement.Controllers.DTO;


namespace TCStudentRecordManagement.Controllers.BLL
{
    public class CohortBLL
    {
        private readonly DataContext _context;

        public CohortBLL(DataContext context)
        {
            _context = context;
        }

        public object AddCohort(string name, DateTime startDate, DateTime endDate)
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

        public object ModifyCohort(CohortDTO cohort)
        {
            // Create a new APIException object to store possible exceptions as checks are performed. 
            APIException exceptionList = new APIException();

            // Check if the id matches a CohortID already in the database.
            if (!CohortExists(cohort.CohortID))
            {
                exceptionList.AddExMessage($"Cohort does not exist with the ID {cohort.CohortID}");
            }
            else
            {
                // Check if the supplied changes meet requirements as for addition
                if (cohort.Name == null || cohort.Name.Trim() == string.Empty)
                {
                    exceptionList.AddExMessage("Name parameter cannot be empty.");
                }
                else
                {
                    // Check if a cohort with the same name exists in the database (trim + case insensitive)
                    // If it does, it has to have a different CohortID from the one supplied (i.e. ignore if they have the same ID)
                    bool dbContainsName = _context.Cohorts.Any(x => (x.Name.ToLower() == cohort.Name.Trim().ToLower()) && (x.CohortID != cohort.CohortID));

                    if (dbContainsName) exceptionList.AddExMessage("A cohort with the same name already exists in the database.");

                    // Check that the startDate is less than endDate
                    bool datesConsistent = cohort.StartDate < cohort.EndDate;
                    if (!datesConsistent) exceptionList.AddExMessage("Start date must be before the end date, check for consistency.");

                }
            }

            if (!exceptionList.HasExceptions)
            {
                // New database name will be trimmed and in Proper Case.
                string dbName = String.Join(" ", cohort.Name.Trim().Split(" ").Select(x => $"{x.Substring(0, 1).ToUpper()}{x.Substring(1)?.ToLower()}").ToArray());

                // Create a new Cohort object
                Cohort newDbObject = new Cohort { CohortID = cohort.CohortID, Name = dbName, StartDate = cohort.StartDate, EndDate = cohort.EndDate };

                // Return the Cohort object (conversion from CohortDTO for internal ops)
                return newDbObject;
            }
            else
            {
                return exceptionList;
            }

        } // End of ModifyCohort

        public bool CohortExists(int id)
        {
            // Returns true if cohort exists with a specified CohortID
            return _context.Cohorts.Any(x => x.CohortID == id);
        }

    }
}
