using System.Collections.Generic;
using System.Linq;
using TCStudentRecordManagement.Controllers.Exceptions;
using TCStudentRecordManagement.Models;
using TCStudentRecordManagement.Models.DTO;

namespace TCStudentRecordManagement.Controllers.BLL
{
    internal class StaffBLL
    {
        private readonly DataContext _context;

        internal StaffBLL(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Business logic for creating staff records in database
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="superUser"</param>
        /// <returns>BLL result object</returns>
        internal object AddStaffBLL(int userID, bool superUser)
        {
            // Create a new APIException object to store possible exceptions as checks are performed. 
            APIException exceptionList = new APIException();

            Dictionary<string, bool> exceptionTests = new Dictionary<string, bool>()
            {
                { "Specified userID does not exist", !_context.Users.Any(x => x.UserID == userID) }
            };

            foreach (KeyValuePair<string, bool> kvp in exceptionTests)
            {
                if (kvp.Value) exceptionList.AddExMessage(kvp.Key);
            }

            if (!exceptionList.HasExceptions)
            {
                StaffDTO newStaff = new StaffDTO
                {
                    UserID = userID,
                    SuperUser = superUser
                };

                return newStaff;
            }
            else
            {
                return exceptionList;
            }

        } // End of AddStaffBLL

        /// <summary>
        /// Business logic for modifying Staff records
        /// </summary>
        /// <param name="staff"></param>
        /// <returns></returns>
        internal object ModifyUserBLL(StaffDTO staff)
        {
            APIException exceptionList = new APIException();

            Dictionary<string, bool> exceptionTests = new Dictionary<string, bool>()
            {
                { "Specified UserID does not exist", !_context.Users.Any(x => x.UserID == staff.UserID) },
                { "Specified StaffID does not exist", !_context.Staff.Any(x => x.StaffID == staff.StaffID) }
            };

            foreach (KeyValuePair<string, bool> kvp in exceptionTests)
            {
                if (kvp.Value) exceptionList.AddExMessage(kvp.Key);
            }

            if (!exceptionList.HasExceptions)
            {
                return staff;
            }
            else
            {
                return exceptionList;
            }

        } // End of ModifyStaff

    } // End of StaffBLL
}
