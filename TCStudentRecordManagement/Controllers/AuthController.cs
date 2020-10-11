using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TCStudentRecordManagement.Controllers
{
    [Route("/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost("google")]
        public async Task<ActionResult> Google(object userData) 
        {

            return Forbid();
        }

    }
}
