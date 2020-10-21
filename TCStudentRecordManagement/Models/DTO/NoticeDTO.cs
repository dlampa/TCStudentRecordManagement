using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCStudentRecordManagement.Models.DTO
{
    /// <summary>
    /// Notice record DTO Class
    /// </summary>
    public class NoticeDTO
    {
        public int NoticeID { get; set; }

        public int CohortID { get; set; }

        public int StaffID { get; set; }

        public DateTime ValidFrom { get; set; }

        public string Markdown { get; set; }

        public string HTML { get; set; }

        public NoticeDTO()
        {

        }

        public NoticeDTO(Notice notice)
        {
            NoticeID = notice.NoticeID;
            CohortID = notice.CohortID;
            StaffID = notice.StaffID;
            ValidFrom = notice.ValidFrom;
            Markdown = notice.Markdown;
            HTML = notice.HTML;
        }
    }
}
