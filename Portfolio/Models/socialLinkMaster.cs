using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portfolio.Models
{
    public class socialLinkMaster
    {
        public int id { get; set; }
        public string platformName { get; set; }
        public string icon { get; set; }
        public string url { get; set; }
        public string status { get; set; }
    }
}