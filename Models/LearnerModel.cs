using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.Models
{
    public class LearnerModel
    {
        public int LearnerId { get; set; }
        public string LearnerName { get; set; }
        public string LearnerEmail { get; set; }
        public string LearnerPassword { get; set; }

        public string LearnerRole { get; set; }

    }
}