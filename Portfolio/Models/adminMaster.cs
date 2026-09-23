using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models
{
    public class adminMaster
    {
        public int id { get; set; }
        [Display(Name = "Name")]
        public string name { get; set; }
        [Display(Name = "User Name")]
        public string username { get; set; }
        [Display(Name = "Email")]
        public string email { get; set; }
        [Display(Name = "Phone")]
        public string phone { get; set; }
        [Display(Name = "Profile Image")]
        public string profileImage { get; set; }
        public HttpPostedFileBase imageFile { get; set; }
        [Display(Name = "Password")]
        public string password { get; set; }
        [Display(Name = "Status")]
        public string status { get; set; }
        [Display(Name = "Date")]
        public string date { get; set; }
    }
}