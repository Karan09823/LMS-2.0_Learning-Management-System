using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.Models
{
    public class CourseData
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string CourseCode { get; set; }
        public string CourseDescription { get; set; }
        public int CourseDuration { get; set; }
        public string CourseCategory { get; set; }
        public string CourseLanguage { get; set; }
    }

    public class ModuleData
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public List<SubModuleData> SubModules { get; set; }
    }

    public class SubModuleData
    {
        public int SubModuleId { get; set; }
        public string SubmoduleName { get; set; }
        public string FileUrl { get; set; }
        public FileData File { get; set; }
    }

    public class FileData
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
        

    }


}