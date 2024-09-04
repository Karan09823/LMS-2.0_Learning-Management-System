using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.Models
{
    public class InstructorModel
    {
        public int InstructorId { get; set; }

        public string InstructorName { get; set; }  

        public string InstructorEmail {  get; set; }

        public string InstructorPassword {  get; set; }
        public string InstructorCourse { get; set; }

        public string InstructorRole { get; set; }

    }
}