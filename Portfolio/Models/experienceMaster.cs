using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models
{
    public class experienceMaster
    {
        public int id { get; set; }
        [Display(Name = "Job Title")]
        public string JobTitle { get; set; }
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }
        [Display(Name = "Location")]
        public string Location { get; set; }
        [Display(Name = "Employment Type")]
        public string employmentType { get; set; }
        [Display(Name = "Start Date")]
        public string StartDate { get; set; }
        [Display(Name = "End Date")]
        public string EndDate { get; set; }
        public bool IsCurrent { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Display(Name = "Technologies")]
        public string Technologies { get; set; }
        [Display(Name = "Company URL")]
        public string CompanyUrl { get; set; }
        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; }
        [Display(Name = "Status")]
        public string Status { get; set; }
    }
}