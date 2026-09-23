using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Portfolio.Models
{
    public class HomeViewModel
    {
            public aboutMaster About { get; set; }

            public DataTable Skills { get; set; }

            public DataTable Services { get; set; }

            public DataTable Experience { get; set; }

            public DataTable Education { get; set; }

            public DataTable Projects { get; set; }

            public DataTable Testimonials { get; set; }

            public DataTable SocialLinks { get; set; }
            public contactMaster Contact { get; set; }

            public settingMaster Settings { get; set; }
        }
    }