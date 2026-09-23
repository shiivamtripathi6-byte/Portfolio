using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Portfolio.Models
{
    public class aboutMaster
    {
        public int id { get; set; }
        public string heading { get; set; }
        public string description { get; set; }
        public string profileImage { get; set; }
        public HttpPostedFileBase imageFile{ get; set; }
        public string designation { get; set; }
        public string experience { get; set; }
        public string status { get; set; }
    }
}