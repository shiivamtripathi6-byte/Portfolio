using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models
{
    public class educationMaster
    {
        public int id { get; set; }
        [Display(Name = "Degree")]
        public string Degree { get; set; }
        [Display(Name = "Institution")]
        public string InstitutionName { get; set; }
        [Display(Name = "Location")]
        public string Location { get; set; }
        [Display(Name = "Start Date")]
        public string StartDate { get; set; }
        [Display(Name = "End Date")]
        public string EndDate { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public string Status { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
    }
}