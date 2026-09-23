using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portfolio.Models
{
    public class skillMaster
    {
        public int id { get; set; }
        public string SkillName { get; set; }
        public string Category { get; set; }
        public string SkillLevel { get; set; }
        public int DisplayOrder { get; set; }
        public string Status { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
    }
}