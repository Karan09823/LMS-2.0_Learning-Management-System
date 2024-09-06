using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.Models
{
    public class Submodule
    {
        public int submoduleID { get; set; }
        public string Title { get; set; }

        public string ImageFilePath { get; set; }

        public string PdfFilePath { get; set; }

        public string AudioFilePath { get; set; }
        public string VideoFilePath { get; set; }

    }
}