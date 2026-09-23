using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models
{
    public class projectMaster
    {
        public int id { get; set; }
        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }
        [Display(Name = "Short Description")]
        public string ShortDescription { get; set; }
        [Display(Name = "Project Image")]
        public string ProjectImage { get; set; }
        public HttpPostedFileBase imageFile { get; set; }
        [Display(Name = "Technologies")]
        public string Technologies { get; set; }
        public string ProjectUrl { get; set; }
        public string GithubUrl { get; set; }
        public string status { get; set; }

    }
}