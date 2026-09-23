using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using Portfolio.Models;

namespace Portfolio.Controllers
{
    public class HomeController : Controller
    {
        DbManager Db = new DbManager();
        [HttpGet]
        public ActionResult Index()
        {
            HomeViewModel model = new HomeViewModel();

                string aboutQuery = @"SELECT * FROM aboutMaster WHERE status != 10 ORDER BY id DESC";
                DataTable aboutTable = Db.GetDataTable(aboutQuery);

               if (aboutTable != null && aboutTable.Rows.Count > 0)
              {
                DataRow row = aboutTable.Rows[0];
                model.About = new aboutMaster();
                model.About.id = Convert.ToInt32(row["id"]);
                model.About.heading = row["heading"].ToString();
                model.About.description = row["description"].ToString();
                model.About.profileImage = row["profileImage"].ToString();
                model.About.designation = row["designation"].ToString();
                model.About.experience = row["experience"].ToString();
                model.About.status = row["status"].ToString();
            }
                string skillQuery = @"SELECT * FROM skillMaster WHERE status != 10 ORDER BY id DESC";
                model.Skills = Db.GetDataTable(skillQuery);

                string serviceQuery = @"SELECT * FROM serviceMaster WHERE status != 10 ORDER BY id DESC";
                model.Services = Db.GetDataTable(serviceQuery);

                string experienceQuery = @"SELECT * FROM experienceMaster WHERE status != 10 ORDER BY id DESC";
                model.Experience = Db.GetDataTable(experienceQuery);

                string educationQuery = @"SELECT * FROM educationMaster WHERE status != 10 ORDER BY id DESC";
                model.Education = Db.GetDataTable(educationQuery);

                string projectQuery = @"SELECT * FROM projectMaster WHERE status != 10 ORDER BY id DESC";
                model.Projects = Db.GetDataTable(projectQuery);

                string testimonialQuery = @"SELECT * FROM testimonialMaster WHERE status != 10 ORDER BY id DESC";
                model.Testimonials = Db.GetDataTable(testimonialQuery);

                string socialQuery = @"SELECT * FROM socialLinkMaster WHERE status != 10 ORDER BY id DESC";
                model.SocialLinks = Db.GetDataTable(socialQuery);

                string settingsQuery = @"SELECT TOP 1 * FROM settingsMaster WHERE status != 10 ORDER BY id DESC";
                DataTable settingsTable = Db.GetDataTable(settingsQuery);

            if (settingsTable != null && settingsTable.Rows.Count > 0)
            {
                DataRow row = settingsTable.Rows[0];

                model.Settings = new settingMaster();

                if (settingsTable.Columns.Contains("id"))
                    model.Settings.id = Convert.ToInt32(row["id"]);

                if (settingsTable.Columns.Contains("siteName"))
                    model.Settings.siteName = row["siteName"].ToString();

                if (settingsTable.Columns.Contains("logo"))
                    model.Settings.logo = row["logo"].ToString();

                if (settingsTable.Columns.Contains("favicon"))
                    model.Settings.favicon = row["favicon"].ToString();

                if (settingsTable.Columns.Contains("email"))
                    model.Settings.email = row["email"].ToString();

                if (settingsTable.Columns.Contains("phone"))
                    model.Settings.phone = row["phone"].ToString();

                if (settingsTable.Columns.Contains("address"))
                    model.Settings.address = row["address"].ToString();

                if (settingsTable.Columns.Contains("resume"))
                    model.Settings.resume = row["resume"].ToString();

                if (settingsTable.Columns.Contains("footerText"))
                    model.Settings.footerText = row["footerText"].ToString();

                if (settingsTable.Columns.Contains("copyrightText"))
                    model.Settings.copyrightText = row["copyrightText"].ToString();

                if (settingsTable.Columns.Contains("status"))
                    model.Settings.status = row["status"].ToString();

                if (settingsTable.Columns.Contains("createdDate"))
                    model.Settings.createdDate = row["createdDate"].ToString();

                if (settingsTable.Columns.Contains("updatedDate"))
                    model.Settings.updatedDate = row["updatedDate"].ToString();
            }
            else
            {
                model.Settings = new settingMaster();
            }
            model.Contact = new contactMaster();
            return View(model);
        }
        public ActionResult AboutMe()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult Services()
        {
            return View();
        }
        public ActionResult Experience()
        {
            return View();
        }
        public ActionResult Skills()
        {
            return View();
        }
        public ActionResult EducationCertificates()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(contactMaster cm)
        {
            int counter = 1;
            try
            {
                object maxId = Db.ExecuteScalar("SELECT ISNULL(MAX(Id), 0) + 1 FROM contactMaster");
                counter = Convert.ToInt32(maxId);
            }
            catch
            {
                counter = Db.TabCounter("Contact");
            }
            if (!ModelState.IsValid)
            {
                return View(cm);
            }

            string insertQuery = @"INSERT INTO contactMaster(id,name, email, phone, subject, message, date, status)VALUES(@id, @name, @email, @phone, @subject, @message, GETDATE(), 1)";

            SqlParameter[] insertParam =
                 {
                    new SqlParameter("@id", counter),
                    new SqlParameter("@name", cm.name ?? (object)DBNull.Value),
                    new SqlParameter("@email", cm.email ?? (object)DBNull.Value),
                    new SqlParameter("@phone", cm.phone ?? (object)DBNull.Value),
                    new SqlParameter("@subject", cm.subject ?? (object)DBNull.Value),
                    new SqlParameter("@message", cm.message ?? (object)DBNull.Value)
                };
            Db.ExecuteNonQuery(insertQuery, insertParam);
            Db.UpdateTabcounter(counter + 1, "contactMaster");
            TempData["Message"] = "Contact Details Saved Successfully.";
            return View();
        }
    }
}