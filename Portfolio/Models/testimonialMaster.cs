using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services.Description;

namespace Portfolio.Models
{
    public class testimonialMaster
    {
        public int id { get; set; }
        public string clientName { get; set; }
        public string designation { get; set; }
        public string companyName { get; set; }
        public string message { get; set; }
        public string clientImage { get; set; }
        public HttpPostedFileBase imageFile { get; set; }
        public int rating { get; set; }
        public int displayOrder { get; set; }
        public string status { get; set; }
    }
}