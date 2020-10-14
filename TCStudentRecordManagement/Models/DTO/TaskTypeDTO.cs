using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCStudentRecordManagement.Models.DTO
{
    /// <summary>
    /// TaskTypeDTO class is used to represent the mutable properties of the TaskType objects/records
    /// </summary>
    public class TaskTypeDTO
    {
        public int TypeID { get; set; }
        public string Description { get; set; }
        public TaskTypeDTO()
        {

        }

        public TaskTypeDTO(TaskType inputTaskType)
        {
            TypeID = inputTaskType.TypeID;
            Description = inputTaskType.Description;
        }
    }



}
