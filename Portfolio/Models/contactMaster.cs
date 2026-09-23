using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models
{
    public class contactMaster
    {
        public int id { get; set; }
        [Display(Name = "Name")]
        public string name { get; set; }
        [Display(Name = "Email")]
        public string email { get; set; }
        [Display(Name = "Phone")]
        public string phone { get; set; }
        [Display(Name = "Subject")]
        public string subject { get; set; }
        [Display(Name = "Message")]
        public string message { get; set; }
        public DateTime date { get; set; }
        public string status { get; set; }
    }
}