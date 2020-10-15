using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCStudentRecordManagement.Models.DTO
{
    /// <summary>
    /// TaskDTO class is used to represent the mutable properties of the Task objects/records
    /// </summary>
    public class TaskDTO
    {
        public int TaskID { get; set; }
        public int UnitID { get; set; }
        public int CohortID { get; set; }
        public int TypeID { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string DocURL { get; set; }
        
        public TaskDTO()
        {

        }

        public TaskDTO(Task inputTask)
        {
            TaskID = inputTask.TaskID;
            UnitID= inputTask.UnitID;
            CohortID = inputTask.CohortID;
            TypeID = inputTask.TypeID;
            Title = inputTask.Title;
            StartDate = inputTask.StartDate;
            EndDate = inputTask.EndDate;
            DocURL = inputTask.DocURL;

    }
    }
}
