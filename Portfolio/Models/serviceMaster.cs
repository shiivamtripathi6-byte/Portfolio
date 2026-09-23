using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models
{
    public class serviceMaster
    {
        public int id { get; set; }
        [Display(Name = "Service Name")]
        public string serviceName{ get; set; }
        [Display(Name = "Description")]
        public string description { get; set; }
        [Display(Name = "Icon")]
        public string icon { get; set; }
        [Display(Name = "Status")]
        public string status { get; set; }
    }
}