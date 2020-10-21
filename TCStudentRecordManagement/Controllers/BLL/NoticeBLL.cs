using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using TCStudentRecordManagement.Controllers.Exceptions;
using TCStudentRecordManagement.Models;
using TCStudentRecordManagement.Models.DTO;

namespace TCStudentRecordManagement.Controllers.BLL
{

    internal class NoticeBLL
    {
        private readonly DataContext _context;

        internal NoticeBLL(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Business logic for retrieving Notice records
        /// </summary>
        /// <param name="noticeID"></param>
        /// <param name="cohortID"></param>
        /// <param name="staffID"></param>
        /// <param name="validFrom"></param>
        /// <param name="isUserStaff"></param>
        /// <returns></returns>
        internal object GetNoticeBLL(int noticeID, int cohortID, int staffID, DateTime validFrom, bool isUserStaff)
        {
            // Create a new APIException object to store possible exceptions as checks are performed. 
            APIException exceptionList = new APIException();

            Dictionary<string, bool> exceptionTests = new Dictionary<string, bool>()
            {
                { "noticeID must be valid", noticeID != 0 && !_context.Notices.Any(x => x.NoticeID == noticeID) },
                { "cohortID must be valid", cohortID != 0 && !_context.Cohorts.Any(x => x.CohortID == cohortID)},
                { "staffID must be valid", staffID != 0 && !_context.Staff.Any(x => x.StaffID == staffID)},
                { "validFrom date cannot be in the future", !(validFrom <= DateTime.Today) && !isUserStaff }
            };

            foreach (KeyValuePair<string, bool> kvp in exceptionTests)
            {
                if (kvp.Value) exceptionList.AddExMessage(kvp.Key);
            }

            if (!exceptionList.HasExceptions)
            {
                // Apply defaults
                if (validFrom == DateTime.MinValue) validFrom = _context.Cohorts.Where(x => x.CohortID == cohortID).First().StartDate;
                

                NoticeDTO getParams = new NoticeDTO
                {
                    NoticeID = noticeID,
                    CohortID = cohortID,
                    StaffID = staffID,
                    ValidFrom = validFrom
                };

                return getParams;
            }
            else
            {
                return exceptionList;
            }

        } // End of GetNoticeBLL

        /// <summary>
        /// Business logic for processing Notice record addition
        /// </summary>
        /// <param name="notice"></param>
        /// <returns></returns>
        internal object AddNoticeBLL(NoticeDTO notice, ClaimsPrincipal userClaims)
        {
            // Get the StaffID of the currently logged in Staff member
            int loggedInStaffID = _context.Users.Where(x => x.Email == userClaims.FindFirst("Email").Value).Include(user => user.StaffData).Select(x => x.StaffData.StaffID).FirstOrDefault();

            // Create a new APIException object to store possible exceptions as checks are performed. 
            APIException exceptionList = new APIException();

            Dictionary<string, bool> exceptionTests = new Dictionary<string, bool>()
            {
                { "cohortID must be valid", notice.CohortID != 0 && !_context.Cohorts.Any(x => x.CohortID == notice.CohortID)},
                { "staffID must be valid", notice.StaffID != 0 && !_context.Staff.Any(x => x.StaffID == notice.StaffID)},
                { "Notice text should not be null or empty", string.IsNullOrEmpty(notice.HTML.Trim()) || string.IsNullOrEmpty(notice.Markdown.Trim()) }
            };

            foreach (KeyValuePair<string, bool> kvp in exceptionTests)
            {
                if (kvp.Value) exceptionList.AddExMessage(kvp.Key);
            }

            if (!exceptionList.HasExceptions)
            {
                notice.StaffID = loggedInStaffID;

               return notice;
            }
            else
            {
                return exceptionList;
            }
            
        } // End of AddNoticeBLL

        /// <summary>
        /// Business logic for processing a modification to a Notice record in the database
        /// </summary>
        /// <param name="notice"></param>
        /// <returns></returns>
        internal object ModifyNoticeBLL(NoticeDTO notice, ClaimsPrincipal userClaims)
        {
            // Get the StaffID of the currently logged in Staff member
            int loggedInStaffID = _context.Users.Where(x => x.Email == userClaims.FindFirst("Email").Value).Include(user => user.StaffData).Select(x => x.StaffData.StaffID).FirstOrDefault();

            // Create a new APIException object to store possible exceptions as checks are performed. 
            APIException exceptionList = new APIException();

            Dictionary<string, bool> exceptionTests = new Dictionary<string, bool>()
            {
                { "CohortID must be valid", notice.CohortID != 0 && !_context.Cohorts.Any(x => x.CohortID == notice.CohortID)},
                { "NoticeID must exist in the database", notice.CohortID != 0 && !_context.Notices.Any(x => x.NoticeID == notice.NoticeID)},
                { "StaffID must be valid", notice.StaffID != 0 && !_context.Staff.Any(x => x.StaffID == notice.StaffID)},
                { "Notice text should not be null or empty", string.IsNullOrEmpty(notice.HTML.Trim()) || string.IsNullOrEmpty(notice.Markdown.Trim()) }
            };

            foreach (KeyValuePair<string, bool> kvp in exceptionTests)
            {
                if (kvp.Value) exceptionList.AddExMessage(kvp.Key);
            }

            if (!exceptionList.HasExceptions)
            {
                notice.StaffID = loggedInStaffID;
                return notice;
            }
            else
            {
                return exceptionList;
            }
            
        } // End of ModifyNoticeBLL

    }

}
