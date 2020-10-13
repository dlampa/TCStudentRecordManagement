using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCStudentRecordManagement.Models;

namespace TCStudentRecordManagement.Controllers.DTO
{
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
