using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace TCStudentRecordManagement
{
    public static class APIConfig
    {
        // Make Configuration available in any part of the application, not just Startup.cs
        // Ref: https://stackoverflow.com/a/55386167/12802214
        public static IConfiguration Configuration;
    }
}
