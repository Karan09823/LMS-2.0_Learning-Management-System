using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.Models
{
    public class ActiveLearnerModel
    {
        public int LearnerId { get; set; }

        public string LearnerName {  get; set; }
        public string LearnerEmail {  get; set; }   
        public int CourseId {  get; set; }
        public string CourseName { get; set;}

        public int OwnerId {  get; set; }   
        public string OwnerName {  get; set; } 
        public string OwnerEmail { get; set; }
    }
}