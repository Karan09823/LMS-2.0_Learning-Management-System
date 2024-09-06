using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.Models
{
    public class AdminModel
    {
       public int Admin_id { get; set; }
       public  string Admin_Name { get; set; }
       public string Admin_Email { get; set; }

        public string Admin_password { get; set; }

        public string Admin_role { get; set; }
    }
}