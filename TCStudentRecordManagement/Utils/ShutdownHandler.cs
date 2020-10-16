using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TCStudentRecordManagement.Models;

namespace TCStudentRecordManagement.Utils
{
    internal class ShutdownHandler
    {
        internal void ShutdownCleanup()
        {
            using (DataContext _context = new DataContext())
            {
                _context.Users.Where(x => x.ActiveToken != null).ToList().ForEach(user => user.ActiveToken = null);
                _context.SaveChanges();
            }
            Utils.Logger.Msg<ShutdownHandler>("Shutdown cleanup actions complete.");
        }
    }
}
