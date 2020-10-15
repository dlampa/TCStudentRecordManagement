using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCStudentRecordManagement.Models.DTO
{
    /// <summary>
    /// UnitDTO class is used to represent the mutable properties of the Unit objects/records
    /// </summary>
    public class UnitDTO
    {
        public int UnitID { get; set; }
        public string Description { get; set; }
        public UnitDTO()
        {

        }

        public UnitDTO(Unit inputUnit)
        {
            UnitID = inputUnit.UnitID;
            Description = inputUnit.Description;
        }
    }
}
