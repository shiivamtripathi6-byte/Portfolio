using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Portfolio.Models
{
    public class settingMaster
    {
        public int id { get; set; }
        [Display(Name = "Site Name")]
        public string siteName { get; set; }
        [Display(Name = "Logo")]
        public string logo { get; set; }
        public HttpPostedFileBase logoFile { get; set; }
        [Display(Name = "FavIcon")]
        public string favicon { get; set; }
        public HttpPostedFileBase faviconFile { get; set; }
        [Display(Name = "Email")]
        public string email { get; set; }
        [Display(Name = "Phone")]
        public string phone { get; set; }
        [Display(Name = "Address")]
        public string address { get; set; }
        [Display(Name = "Resume")]
        public string resume { get; set; }
        public HttpPostedFileBase resumeFile { get; set; }
        [Display(Name = "Footer Text")]
        public string footerText { get; set; }
        [Display(Name = "CopyRight")]
        public string copyrightText { get; set; }
        [Display(Name = "Status")]
        public string status { get; set; }
        [Display(Name = "Date")]
        public string createdDate { get; set; }
        [Display(Name = "Updated Date")]
        public string updatedDate { get; set; }
    }
}