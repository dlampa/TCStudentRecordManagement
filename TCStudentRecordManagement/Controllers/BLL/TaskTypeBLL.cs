using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCStudentRecordManagement.Models;

namespace TCStudentRecordManagement.Controllers.BLL
{
    public class TaskTypeBLL
    {
        private readonly DataContext _context;

        public TaskTypeBLL(DataContext context)
        {
            _context = context;
        }
    }
}
