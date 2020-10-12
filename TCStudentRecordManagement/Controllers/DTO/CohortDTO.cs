using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCStudentRecordManagement.Models;

namespace TCStudentRecordManagement.Controllers.DTO
{
    public class CohortDTO
    {
        public string Name { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public CohortDTO()
        {

        }

        public CohortDTO(Cohort inputCohort)
        {
            Name = inputCohort.Name;
            StartDate = inputCohort.StartDate;
            EndDate = inputCohort.EndDate;
        }
    }
}
