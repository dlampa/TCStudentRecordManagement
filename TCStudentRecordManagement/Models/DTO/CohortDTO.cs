using System;
using TCStudentRecordManagement.Models;

namespace TCStudentRecordManagement.Models.DTO
{
    /// <summary>
    /// CohortDTO class is used to encapsulate the Cohort object in a form appropriate for add, modify and get API targets
    /// </summary>
    public class CohortDTO
    {
        public int CohortID { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public CohortDTO()
        {

        }

        public CohortDTO(Cohort inputCohort)
        {
            if (inputCohort != null)
            {
                CohortID = inputCohort.CohortID;
                Name = inputCohort.Name;
                StartDate = inputCohort.StartDate;
                EndDate = inputCohort.EndDate;
            }
        }

    }
}
