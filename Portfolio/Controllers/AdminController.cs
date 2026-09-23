using Microsoft.Ajax.Utilities;
using Portfolio.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Web.UI.WebControls;

namespace Portfolio.Controllers
{
    public class AdminController : Controller
    {
        DbManager Db = new DbManager();
        // GET: Admin
        [HttpGet]
         public ActionResult Dashboard()
            {
                try
                {
                    ViewBag.ProjectCount = GetCount("projectMaster");
                    ViewBag.SkillCount = GetCount("skillMaster");
                    ViewBag.ExperienceCount = GetCount("experienceMaster");
                    ViewBag.EducationCount = GetCount("educationMaster");
                    ViewBag.ServiceCount = GetCount("serviceMaster");
                    ViewBag.AboutCount = GetCount("aboutMaster");

                    // Total Contact Messages
                    ViewBag.MessageCount = GetCount("contactMaster");

                    // Unread Contact Messages
                    ViewBag.UnreadMessageCount =
                        GetCount("contactMaster", "status = 0");

                    return View();
                }
                catch (Exception ex)
                {
                    ViewBag.ProjectCount = 0;
                    ViewBag.SkillCount = 0;
                    ViewBag.ExperienceCount = 0;
                    ViewBag.EducationCount = 0;
                    ViewBag.ServiceCount = 0;
                    ViewBag.AboutCount = 0;
                    ViewBag.MessageCount = 0;
                    ViewBag.UnreadMessageCount = 0;

                    ViewBag.Error = ex.Message;

                    return View();
                }
            }
        private int GetCount(string tableName, string condition = null)
            {
                string query;

                if (string.IsNullOrEmpty(condition))
                {
                    query = "SELECT COUNT(*) FROM " + tableName +
                            " WHERE status != 10";
                }
                else
                {
                    query = "SELECT COUNT(*) FROM " + tableName +
                            " WHERE " + condition;
                }

                DataTable dt = Db.GetDataTable(query);

                if (dt != null && dt.Rows.Count > 0)
                {
                    return Convert.ToInt32(dt.Rows[0][0]);
                }

                return 0;
            }
        [HttpGet]
        public ActionResult Projects(string id)
        {
            projectMaster pm = new projectMaster();

            if (!string.IsNullOrEmpty(id))
            {
                string query = @"SELECT * FROM ProjectMaster WHERE Id = @id AND status != 10";

                SqlParameter[] param =
                {
                   new SqlParameter("@id", id)
                };

                DataTable dt = Db.GetDataTable(query, param);

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    pm.id = Convert.ToInt32(row["Id"]);
                    pm.ProjectName = row["ProjectName"] == DBNull.Value ? "" : row["ProjectName"].ToString();
                    pm.ShortDescription = row["ShortDescription"] == DBNull.Value ? "" : row["ShortDescription"].ToString();
                    pm.ProjectImage = row["ProjectImage"] == DBNull.Value ? "" : row["ProjectImage"].ToString();
                    pm.Technologies = row["Technologies"] == DBNull.Value ? "" : row["Technologies"].ToString();
                    pm.ProjectUrl = row["ProjectUrl"] == DBNull.Value ? "" : row["ProjectUrl"].ToString();
                    pm.GithubUrl = row["GithubUrl"] == DBNull.Value ? "" : row["GithubUrl"].ToString();
                }
            }

            return View(pm);
        }
        [HttpPost]
        public ActionResult Projects(projectMaster pm, HttpPostedFileBase imageFile)
        {
            try
            {
                string imagePath = pm.ProjectImage;

                // ================= IMAGE UPLOAD =================
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    string extension = Path.GetExtension(imageFile.FileName);

                    string fileName = "ProjectImage_"
                        + DateTime.Now.ToString("yyyyMMddHHmmssfff")
                        + extension;

                    string folderPath = Server.MapPath("~/Content/Upload/Project/");

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    string fullPath = Path.Combine(folderPath, fileName);

                    imageFile.SaveAs(fullPath);

                    imagePath = "~/Content/Upload/Project/" + fileName;
                }
                // ================= EDIT =================
                if (pm.id > 0)
                {
                    string updateQuery = @"
                UPDATE ProjectMaster
                SET
                    ProjectName = @ProjectName,
                    ShortDescription = @ShortDescription,
                    ProjectImage = @ProjectImage,
                    Technologies = @Technologies,
                    ProjectUrl = @ProjectUrl,
                    GithubUrl = @GithubUrl
                WHERE Id = @id";

                    SqlParameter[] updateParam =
                    {
                new SqlParameter("@id", pm.id),

                new SqlParameter("@ProjectName",
                    string.IsNullOrEmpty(pm.ProjectName)
                    ? (object)DBNull.Value
                    : pm.ProjectName),

                new SqlParameter("@ShortDescription",
                    string.IsNullOrEmpty(pm.ShortDescription)
                    ? (object)DBNull.Value
                    : pm.ShortDescription),

                new SqlParameter("@ProjectImage",
                    string.IsNullOrEmpty(imagePath)
                    ? (object)DBNull.Value
                    : imagePath),

                new SqlParameter("@Technologies",
                    string.IsNullOrEmpty(pm.Technologies)
                    ? (object)DBNull.Value
                    : pm.Technologies),

                new SqlParameter("@ProjectUrl",
                    string.IsNullOrEmpty(pm.ProjectUrl)
                    ? (object)DBNull.Value
                    : pm.ProjectUrl),

                new SqlParameter("@GithubUrl",
                    string.IsNullOrEmpty(pm.GithubUrl)
                    ? (object)DBNull.Value
                    : pm.GithubUrl)
            };

                    Db.ExecuteNonQuery(updateQuery, updateParam);

                    TempData["Message"] = "Project Updated Successfully.";

                    return RedirectToAction("ShowProjects", "Admin");
                }

                // ================= ADD NEW PROJECT =================

                int counter = 1;

                try
                {
                    object maxId = Db.ExecuteScalar(
                        "SELECT ISNULL(MAX(Id), 0) + 1 FROM ProjectMaster"
                    );

                    counter = Convert.ToInt32(maxId);
                }
                catch
                {
                    counter = Db.TabCounter("ProjectMaster");
                }

                string insertQuery = @"
            INSERT INTO ProjectMaster
            (
                Id,
                ProjectName,
                ShortDescription,
                ProjectImage,
                Technologies,
                ProjectUrl,
                GithubUrl,
                status
            )
            VALUES
            (
                @id,
                @ProjectName,
                @ShortDescription,
                @ProjectImage,
                @Technologies,
                @ProjectUrl,
                @GithubUrl,
                1
            )";

                SqlParameter[] insertParam =
                {
            new SqlParameter("@id", counter),

            new SqlParameter("@ProjectName",
                string.IsNullOrEmpty(pm.ProjectName)
                ? (object)DBNull.Value
                : pm.ProjectName),

            new SqlParameter("@ShortDescription",
                string.IsNullOrEmpty(pm.ShortDescription)
                ? (object)DBNull.Value
                : pm.ShortDescription),

            new SqlParameter("@ProjectImage",
                string.IsNullOrEmpty(imagePath)
                ? (object)DBNull.Value
                : imagePath),

            new SqlParameter("@Technologies",
                string.IsNullOrEmpty(pm.Technologies)
                ? (object)DBNull.Value
                : pm.Technologies),

            new SqlParameter("@ProjectUrl",
                string.IsNullOrEmpty(pm.ProjectUrl)
                ? (object)DBNull.Value
                : pm.ProjectUrl),

            new SqlParameter("@GithubUrl",
                string.IsNullOrEmpty(pm.GithubUrl)
                ? (object)DBNull.Value
                : pm.GithubUrl)
        };

                Db.ExecuteNonQuery(insertQuery, insertParam);

                Db.UpdateTabcounter(counter + 1, "ProjectMaster");

                TempData["Message"] = "Project Saved Successfully.";

                return RedirectToAction("ShowProjects", "Admin");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;

                return View(pm);
            }
        }
        [HttpGet]
        public ActionResult DeleteProjects(int id)
        {
            try
            {
                string deleteQuery = @"UPDATE ProjectMaster SET status = 10 WHERE Id = @id";

                SqlParameter[] deleteParam =
                {
                     new SqlParameter("@id", id)
                };

                Db.ExecuteNonQuery(deleteQuery, deleteParam);

                TempData["Message"] = "Project Deleted Successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("ShowProjects", "Admin");
        }
        [HttpPost]
        public ActionResult ToggleProjectStatus(string id)
        {
            string sql = @"UPDATE ProjectMaster SET Status = CASE WHEN Status = '1' THEN '0' ELSE '1' END WHERE id = @Id";

            SqlParameter[] p =
            {
                new SqlParameter("@Id", id)
            };

            Db.ExecuteNonQuery(sql, p);

            TempData["Message"] = "Status updated successfully!";

            return RedirectToAction("ShowProjects", "Admin");
        }
        [HttpGet]
        public ActionResult ShowProjects()
        {
            DataTable dt = new DataTable();

            try
            {
                string query = "SELECT * FROM projectMaster WHERE status <> 10 ORDER BY id DESC";

                dt = Db.GetDataTable(query);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return View(dt);
        }
        [HttpGet]
        public ActionResult ManageSkills(string id)
        {
            skillMaster sm = new skillMaster();
            if (!string.IsNullOrEmpty(id))
            {
                string query = "SELECT * FROM skillMaster WHERE Id = @id AND status != 10";
                SqlParameter[] param =
                {
                    new SqlParameter("@id", id)
                };
                DataTable dt = Db.GetDataTable(query, param);
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    sm.id = Convert.ToInt32(row["Id"]);
                    sm.SkillName = row["SkillName"] == DBNull.Value ? "" : row["SkillName"].ToString();
                    sm.Category = row["Category"] == DBNull.Value ? "" : row["Category"].ToString();
                    sm.SkillLevel = row["SkillLevel"] == DBNull.Value ? "" : row["SkillLevel"].ToString();
                    sm.DisplayOrder = row["DisplayOrder"] != DBNull.Value ? Convert.ToInt32(row["DisplayOrder"]) : 0;
                    sm.Status = row["Status"] == DBNull.Value ? "" : row["Status"].ToString().Trim();
                    sm.CreatedDate = row["CreatedDate"] == DBNull.Value ? "" : row["CreatedDate"].ToString();
                    sm.UpdatedDate = row["UpdatedDate"] == DBNull.Value ? "" : row["UpdatedDate"].ToString();
                }
            }
            return View(sm);
        }
        [HttpPost]
        public ActionResult ManageSkills(skillMaster sm)
        {
            try
            {
                // EDIT
                if (sm.id > 0)
                {
                    string updateQuery = @"UPDATE skillMaster SET SkillName = @SkillName, Category = @Category, SkillLevel = @SkillLevel, DisplayOrder = @DisplayOrder, UpdatedDate = @UpdatedDate WHERE Id = @id";
                    SqlParameter[] updateParam =
                    {
                        new SqlParameter("@id", sm.id),
                        new SqlParameter("@SkillName", sm.SkillName ?? (object)DBNull.Value),
                        new SqlParameter("@Category", sm.Category ?? (object)DBNull.Value),
                        new SqlParameter("@SkillLevel", sm.SkillLevel ?? (object)DBNull.Value),
                        new SqlParameter("@DisplayOrder", sm.DisplayOrder),
                        new SqlParameter("@UpdatedDate", DateTime.Now)
                    };

                    Db.ExecuteNonQuery(updateQuery, updateParam);
                    TempData["Message"] = "Skills Updated Successfully.";
                    return RedirectToAction("ShowSkills", "Admin");
                }
                // ADD NEW PROJECT
                int counter = 1;
                try
                {
                    object maxId = Db.ExecuteScalar("SELECT ISNULL(MAX(Id), 0) + 1 FROM skillMaster");
                    counter = Convert.ToInt32(maxId);
                }
                catch
                {
                    counter = Db.TabCounter("skillMaster");
                }
                string insertQuery = @"INSERT INTO skillMaster(Id, SkillName, Category, SkillLevel, DisplayOrder, CreatedDate, UpdatedDate, Status)VALUES(@id, @SkillName, @Category, @SkillLevel, @DisplayOrder, GETDATE(), @UpdatedDate, 1)";

                SqlParameter[] insertParam =
                {
                   new SqlParameter("@id", counter),
                   new SqlParameter("@SkillName", sm.SkillName ?? (object)DBNull.Value),
                   new SqlParameter("@Category", sm.Category ?? (object)DBNull.Value),
                   new SqlParameter("@SkillLevel", sm.SkillLevel ?? (object)DBNull.Value),
                   new SqlParameter("@DisplayOrder", sm.DisplayOrder),
                   new SqlParameter("@CreatedDate", DateTime.Now),
                   new SqlParameter("@UpdatedDate", sm.UpdatedDate ?? (object)DBNull.Value)
               };

                Db.ExecuteNonQuery(insertQuery, insertParam);

                Db.UpdateTabcounter(counter + 1, "skillMaster");

                TempData["Message"] = "Skills Saved Successfully.";

                return RedirectToAction("ShowSkills", "Admin");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(sm);
            }
        }
        [HttpGet]
        public ActionResult ShowSkills()
        {
            string query = @"SELECT * FROM skillMaster WHERE status != 10 ORDER BY DisplayOrder ASC, Id DESC";
            DataTable dt = Db.GetDataTable(query);
            List<skillMaster> skills = new List<skillMaster>();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    skillMaster sm = new skillMaster();

                    sm.id = row["Id"] == DBNull.Value
                        ? 0
                        : Convert.ToInt32(row["Id"]);

                    sm.SkillName = row["SkillName"] == DBNull.Value
                        ? ""
                        : row["SkillName"].ToString();

                    sm.Category = row["Category"] == DBNull.Value
                        ? ""
                        : row["Category"].ToString();

                    sm.SkillLevel = row["SkillLevel"] == DBNull.Value
                        ? ""
                        : row["SkillLevel"].ToString();

                    sm.DisplayOrder = row["DisplayOrder"] == DBNull.Value
                        ? 0
                        : Convert.ToInt32(row["DisplayOrder"]);

                    sm.Status = row["Status"] == DBNull.Value
                        ? ""
                        : row["Status"].ToString().Trim();

                    sm.CreatedDate = row["CreatedDate"] == DBNull.Value
                        ? ""
                        : row["CreatedDate"].ToString();

                    sm.UpdatedDate = row["UpdatedDate"] == DBNull.Value
                        ? ""
                        : row["UpdatedDate"].ToString();

                    skills.Add(sm);
                }
            }

            return View(skills);
        }

        [HttpGet]
        public ActionResult DeleteSkills(int id)
        {
            try
            {
                string deleteQuery = @"UPDATE skillMaster SET status = 10 WHERE Id = @id";

                SqlParameter[] deleteParam =
                {
                     new SqlParameter("@id", id)
                };

                Db.ExecuteNonQuery(deleteQuery, deleteParam);

                TempData["Message"] = "Skills Deleted Successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("ShowSkills", "Admin");
        }
        [HttpPost]
        public ActionResult ToggleSkillStatus(string id)
        {
            string sql = @"UPDATE skillMaster SET Status = CASE WHEN Status = '1' THEN '0' ELSE '1' END WHERE id = @Id";

            SqlParameter[] p =
            {
                new SqlParameter("@Id", id)
            };

            Db.ExecuteNonQuery(sql, p);

            TempData["Message"] = "Status updated successfully!";

            return RedirectToAction("ShowSkills", "Admin");
        }
        [HttpGet]
        public ActionResult ManageExperience(string id)
        {
            experienceMaster em = new experienceMaster();
            if (!string.IsNullOrEmpty(id))
            {
                string query = @"SELECT * FROM experienceMaster WHERE Id = @id AND status != 10";
                SqlParameter[] param =
                {
                   new SqlParameter("@id", id)
                };
                DataTable dt = Db.GetDataTable(query, param);
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    em.id = Convert.ToInt32(row["Id"]);
                    em.JobTitle = row["JobTitle"] == DBNull.Value ? "" : row["JobTitle"].ToString();
                    em.CompanyName = row["CompanyName"] == DBNull.Value ? "" : row["CompanyName"].ToString();
                    em.Location = row["Location"] == DBNull.Value ? "" : row["Location"].ToString();
                    em.employmentType = row["employmentType"] == DBNull.Value ? "" : row["employmentType"].ToString();
                    em.StartDate = row["StartDate"] == DBNull.Value ? "" : row["StartDate"].ToString();
                    em.EndDate = row["EndDate"] == DBNull.Value ? "" : row["EndDate"].ToString();
                    em.IsCurrent = row["IsCurrent"] != DBNull.Value && Convert.ToBoolean(row["IsCurrent"]);
                    em.Description = row["Description"] == DBNull.Value ? "" : row["Description"].ToString();
                    em.Technologies = row["Technologies"] == DBNull.Value ? "" : row["Technologies"].ToString();
                    em.CompanyUrl = row["CompanyUrl"] == DBNull.Value ? "" : row["CompanyUrl"].ToString();
                    if (row["DisplayOrder"] != DBNull.Value)
                    {
                        em.DisplayOrder = Convert.ToInt32(row["DisplayOrder"]);
                    }
                }
            }
            return View(em);
        }
        [HttpPost]
        public ActionResult ManageExperience(experienceMaster em)
        {
            if (em.id > 0)
            {
                string updateQuery = @"UPDATE experienceMaster SET JobTitle = @JobTitle, CompanyName = @CompanyName, Location = @Location, employmentType = @employmentType, StartDate = @StartDate, EndDate = @EndDate, IsCurrent = @IsCurrent, Description = @Description, Technologies = @Technologies, CompanyUrl = @CompanyUrl, DisplayOrder = @DisplayOrder WHERE Id = @id";
                SqlParameter[] updateParam =
                {
                        new SqlParameter("@id", em.id),
                        new SqlParameter("@JobTitle",em.JobTitle ?? (object)DBNull.Value),
                        new SqlParameter("@CompanyName", em.CompanyName ?? (object)DBNull.Value),
                        new SqlParameter("@Location", em.Location ?? (object)DBNull.Value),
                        new SqlParameter("@employmentType", em.employmentType ?? (object)DBNull.Value),
                        new SqlParameter("@StartDate", em.StartDate ?? (object)DBNull.Value),
                        new SqlParameter("@EndDate", em.EndDate ?? (object)DBNull.Value),
                        new SqlParameter("@IsCurrent", em.IsCurrent),
                        new SqlParameter("@Description", em.Description ?? (object)DBNull.Value),
                        new SqlParameter("@Technologies", em.Technologies ?? (object)DBNull.Value),
                        new SqlParameter("@CompanyUrl", em.CompanyUrl ?? (object)DBNull.Value),
                        new SqlParameter("@DisplayOrder", em.DisplayOrder)
                   };

                Db.ExecuteNonQuery(updateQuery, updateParam);
                TempData["Message"] = "Experience Updated Successfully.";
                return RedirectToAction("ShowExperience", "Admin");
            }
            int counter = 1;
            try
            {
                object maxId = Db.ExecuteScalar("SELECT ISNULL(MAX(Id), 0) + 1 FROM experienceMaster");
                counter = Convert.ToInt32(maxId);
            }
            catch
            {
                counter = Db.TabCounter("experienceMaster");
            }
            string insertQuery = @"INSERT INTO experienceMaster(id, JobTitle, CompanyName, Location, employmentType, StartDate, EndDate, IsCurrent, Description, Technologies, CompanyUrl, DisplayOrder, Status)VALUES(@id, @JobTitle, @CompanyName, @Location, @employmentType, @StartDate, @EndDate, @IsCurrent, @Description, @Technologies, @CompanyUrl, @DisplayOrder, 1)";

            SqlParameter[] insertParam =
            {
                new SqlParameter("id", counter),
                new SqlParameter("JobTitle", em.JobTitle ?? (object)DBNull.Value),
                new SqlParameter("CompanyName", em.CompanyName ?? (object)DBNull.Value),
                new SqlParameter("Location", em.Location ?? (object)DBNull.Value),
                new SqlParameter("employmentType", em.employmentType ?? (object)DBNull.Value),
                new SqlParameter("StartDate", em.StartDate ?? (object)DBNull.Value),
                new SqlParameter("EndDate", em.EndDate ?? (object)DBNull.Value),
                new SqlParameter("@IsCurrent", em.IsCurrent),
                new SqlParameter("Description", em.Description ?? (object)DBNull.Value),
                new SqlParameter("Technologies", em.Technologies ?? (object)DBNull.Value),
                new SqlParameter("CompanyUrl", em.CompanyUrl ?? (object)DBNull.Value),
                new SqlParameter("DisplayOrder", em.DisplayOrder)
            };
            Db.ExecuteNonQuery(insertQuery, insertParam);
            Db.UpdateTabcounter(counter + 1, "experienceMaster");
            TempData["Message"] = "experience Saved Successfully.";
            return RedirectToAction("ShowExperience", "Admin");
        }
        [HttpPost]
        public ActionResult ToggleExperienceStatus(string id)
        {
            string sql = @"UPDATE experienceMaster SET Status = CASE WHEN Status = '1' THEN '0' ELSE '1' END WHERE id = @Id";

            SqlParameter[] p =
            {
                new SqlParameter("@Id", id)
            };

            Db.ExecuteNonQuery(sql, p);

            TempData["Message"] = "Status updated successfully!";

            return RedirectToAction("ShowExperience", "Admin");
        }
        [HttpGet]
        public ActionResult ShowExperience()
        {
            string query = @"SELECT * FROM experienceMaster WHERE status != 10 ORDER BY DisplayOrder ASC, StartDate DESC";
            DataTable dt = Db.GetDataTable(query);
            List<experienceMaster> list = new List<experienceMaster>();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    experienceMaster item = new experienceMaster();

                    item.id = Convert.ToInt32(row["id"]);
                    item.JobTitle = row["JobTitle"].ToString();
                    item.CompanyName = row["CompanyName"].ToString();
                    item.Location = row["Location"].ToString();
                    item.employmentType = row["employmentType"].ToString();
                    item.StartDate = row["StartDate"] == DBNull.Value ? "" : row["StartDate"].ToString();
                    item.EndDate = row["EndDate"] == DBNull.Value ? "" : row["EndDate"].ToString();
                    item.IsCurrent = row["IsCurrent"] != DBNull.Value && Convert.ToBoolean(row["IsCurrent"]);
                    item.Description = row["Description"].ToString();
                    item.Technologies = row["Technologies"].ToString();
                    item.CompanyUrl = row["CompanyUrl"].ToString();
                    if (row["DisplayOrder"] != DBNull.Value)
                        item.DisplayOrder = Convert.ToInt32(row["DisplayOrder"]);
                    // IMPORTANT
                    item.Status = row["Status"] == DBNull.Value
                        ? ""
                        : row["Status"].ToString().Trim();

                    list.Add(item);
                }
            }

            return View(list);
        }
        [HttpGet]
        public ActionResult DeleteExperience(int id)
        {
            try
            {
                string query = @"UPDATE experienceMaster SET status = 10 WHERE id = @id";

                SqlParameter[] param =
                {
                      new SqlParameter("@id", id)
                };

                Db.ExecuteNonQuery(query, param);

                TempData["Message"] = "Experience deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error while deleting experience: " + ex.Message;
            }

            return RedirectToAction("ShowExperience", "Admin");
        }
        [HttpGet]
        public ActionResult educationMaster(string id)
        {
            educationMaster em = new educationMaster();
            if (!string.IsNullOrEmpty(id))
            {
                string query = @"SELECT * FROM educationMaster WHERE id = @id AND status != 10";
                SqlParameter[] param =
                {
                    new SqlParameter("@id", id)
                };
                DataTable dt = Db.GetDataTable(query, param);
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    em.id = Convert.ToInt32(row["id"]);
                    em.Degree = row["Degree"] == DBNull.Value ? "" : row["Degree"].ToString();
                    em.InstitutionName = row["InstitutionName"] == DBNull.Value ? "" : row["InstitutionName"].ToString();
                    em.Location = row["Location"] == DBNull.Value ? "" : row["Location"].ToString();
                    em.StartDate = row["StartDate"] == DBNull.Value ? "" : row["StartDate"].ToString();
                    em.EndDate = row["EndDate"] == DBNull.Value ? "" : row["EndDate"].ToString();
                    em.Description = row["Description"] == DBNull.Value ? "" : row["Description"].ToString();
                    if (row["DisplayOrder"] != DBNull.Value)
                        em.DisplayOrder = Convert.ToInt32(row["DisplayOrder"]);
                    // IMPORTANT
                    em.Status = row["Status"] == DBNull.Value ? "" : row["Status"].ToString().Trim();
                }
            }
            return View(em);
        }
        [HttpPost]
        public ActionResult educationMaster(educationMaster em)
        {
            if (em.id > 0)
            {
                string updateQuery = @"UPDATE educationMaster SET Degree = @Degree, InstitutionName = @InstitutionName, Location = @Location, StartDate = @StartDate, EndDate = @EndDate, Description = @Description, DisplayOrder = @DisplayOrder, UpdatedDate = @UpdatedDate WHERE Id = @id";
                SqlParameter[] updateParam =
                {
                        new SqlParameter("@id", em.id),
                        new SqlParameter("@Degree",em.Degree ?? (object)DBNull.Value),
                        new SqlParameter("@InstitutionName", em.InstitutionName ?? (object)DBNull.Value),
                        new SqlParameter("@Location", em.Location ?? (object)DBNull.Value),
                        new SqlParameter("@StartDate", em.StartDate ?? (object)DBNull.Value),
                        new SqlParameter("@EndDate", em.EndDate ?? (object)DBNull.Value),
                        new SqlParameter("@Description", em.Description ?? (object)DBNull.Value),
                        new SqlParameter("@DisplayOrder", em.DisplayOrder),
                        new SqlParameter("@UpdatedDate", DateTime.Now)
                   };

                Db.ExecuteNonQuery(updateQuery, updateParam);
                TempData["Message"] = "Education Updated Successfully.";
                return RedirectToAction("ShowEducation", "Admin");
            }
            int counter = 1;
            try
            {
                object maxId = Db.ExecuteScalar("SELECT ISNULL(MAX(Id), 0) + 1 FROM educationMaster");
                counter = Convert.ToInt32(maxId);
            }
            catch
            {
                counter = Db.TabCounter("educationMaster");
            }
            string insertQuery = @"INSERT INTO educationMaster(id, Degree, InstitutionName, Location, StartDate, EndDate, Description, DisplayOrder, Status, CreatedDate, UpdatedDate)VALUES(@id, @Degree, @InstitutionName, @Location, @StartDate, @EndDate, @Description, @DisplayOrder, 1, GETDATE(), @UpdatedDate)";

            SqlParameter[] insertParam =
            {
                new SqlParameter("id", counter),
                new SqlParameter("Degree", em.Degree ?? (object)DBNull.Value),
                new SqlParameter("InstitutionName", em.InstitutionName ?? (object)DBNull.Value),
                new SqlParameter("Location", em.Location ?? (object)DBNull.Value),
                new SqlParameter("StartDate", em.StartDate ?? (object)DBNull.Value),
                new SqlParameter("EndDate", em.EndDate ?? (object)DBNull.Value),
                new SqlParameter("Description", em.Description ?? (object)DBNull.Value),
                new SqlParameter("DisplayOrder", em.DisplayOrder),
                new SqlParameter("@CreatedDate", DateTime.Now),
                new SqlParameter("@UpdatedDate", em.UpdatedDate ?? (object)DBNull.Value)
            };
            Db.ExecuteNonQuery(insertQuery, insertParam);
            Db.UpdateTabcounter(counter + 1, "educationMaster");
            TempData["Message"] = "education Saved Successfully.";
            return RedirectToAction("ShowEducation", "Admin");
        }
        [HttpGet]
        public ActionResult DeleteEducation(int id)
        {
            string query = @"UPDATE educationMaster SET status = 10 WHERE id = @id";

            SqlParameter[] param =
            {
                new SqlParameter("@id", id)
            };

            Db.ExecuteNonQuery(query, param);
            TempData["Message"] = "Education deleted successfully";
            return RedirectToAction("ShowEducation", "Admin");
        }
        [HttpGet]
        public ActionResult ShowEducation()
        {
            string query = @"SELECT * FROM educationMaster WHERE status != 10 ORDER BY DisplayOrder ASC, StartDate DESC";
            DataTable dt = Db.GetDataTable(query);
            List<educationMaster> list = new List<educationMaster>();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    educationMaster item = new educationMaster();
                    item.id = Convert.ToInt32(row["id"]);
                    item.Degree = row["Degree"].ToString();
                    item.InstitutionName = row["InstitutionName"].ToString();
                    item.Location = row["Location"].ToString();
                    item.StartDate = row["StartDate"] == DBNull.Value ? "" : row["StartDate"].ToString();
                    item.EndDate = row["EndDate"] == DBNull.Value ? "" : row["EndDate"].ToString();
                    item.Description = row["Description"].ToString();
                    if (row["DisplayOrder"] != DBNull.Value)
                        item.DisplayOrder = Convert.ToInt32(row["DisplayOrder"]);
                    // IMPORTANT
                    item.Status = row["Status"] == DBNull.Value ? "" : row["Status"].ToString().Trim();
                    list.Add(item);
                }
            }
            return View(list);
        }
        [HttpPost]
        public ActionResult ToggleEducationStatus(string id)
        {
            string sql = @"UPDATE educationMaster SET Status = CASE WHEN Status = '1' THEN '0' ELSE '1' END WHERE id = @Id";
            SqlParameter[] p =
            {
                new SqlParameter("@Id", id)
            };
            Db.ExecuteNonQuery(sql, p);
            TempData["Message"] = "Status updated successfully!";
            return RedirectToAction("ShowEducation", "Admin");
        }
        [HttpGet]
        public ActionResult AboutMaster(string id)
        {
            aboutMaster am = new aboutMaster();
            if (!string.IsNullOrEmpty(id))
            {
                string query = @"SELECT * FROM aboutMaster WHERE id = @id AND status != 10";
                SqlParameter[] param =
                {
                    new SqlParameter("@id", id)
                };
                DataTable dt = Db.GetDataTable(query, param);
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    am.id = Convert.ToInt32(row["id"]);
                    am.heading = row["heading"] == DBNull.Value ? "" : row["heading"].ToString();
                    am.description = row["description"] == DBNull.Value ? "" : row["description"].ToString();
                    am.profileImage = row["profileImage"] == DBNull.Value ? "" : row["profileImage"].ToString();
                    am.designation = row["designation"] == DBNull.Value ? "" : row["designation"].ToString();
                    am.experience = row["experience"] == DBNull.Value ? "" : row["experience"].ToString();
                    am.status = row["status"] == DBNull.Value ? "" : row["status"].ToString().Trim();
                }
            }
            return View(am);
        }
        [HttpPost]
        public ActionResult AboutMaster(aboutMaster am, HttpPostedFileBase imageFile)
        {
            try
            {
                string imagePath = am.profileImage;

                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    string uploadDir = Server.MapPath("~/Content/Upload/About/");

                    if (!Directory.Exists(uploadDir))
                    {
                        Directory.CreateDirectory(uploadDir);
                    }

                    string fileExtension = Path.GetExtension(imageFile.FileName);
                    string uniqueFileName = DateTime.Now.Ticks + fileExtension;
                    string filePath = Path.Combine(uploadDir, uniqueFileName);
                    imageFile.SaveAs(filePath);
                    imagePath = "/Content/Upload/About/" + uniqueFileName;
                }

                // =========================
                // UPDATE
                // =========================
                if (am.id > 0)
                {
                    string updateQuery = @"UPDATE aboutMaster SET heading = @heading, description = @description, profileImage = @profileImage, designation = @designation, experience = @experience WHERE id = @id";

                    SqlParameter[] updateParam =
                    {
                       new SqlParameter("@id", am.id),
                       new SqlParameter("@heading", am.heading ?? (object)DBNull.Value),
                       new SqlParameter("@description", am.description ?? (object)DBNull.Value),
                       new SqlParameter("@profileImage", imagePath ?? (object)DBNull.Value),
                       new SqlParameter("@designation", am.designation ?? (object)DBNull.Value),
                       new SqlParameter("@experience", am.experience ?? (object)DBNull.Value)
                   };

                    Db.ExecuteNonQuery(updateQuery, updateParam);
                    TempData["Message"] = "About Details Updated Successfully.";
                    return RedirectToAction("ShowAbout", "Admin");
                }
                int counter = 1;

                try
                {
                    object maxId = Db.ExecuteScalar(
                        "SELECT ISNULL(MAX(Id), 0) + 1 FROM aboutMaster");

                    counter = Convert.ToInt32(maxId);
                }
                catch
                {
                    counter = Db.TabCounter("aboutMaster");
                }
                string insertQuery = @"INSERT INTO aboutMaster(id, heading, description, profileImage, designation, experience, status)VALUES( @id, @heading, @description, @profileImage, @designation, @experience, 1)";

                SqlParameter[] insertParam =
                {
                    new SqlParameter("@id", counter),
                    new SqlParameter("@heading", am.heading ?? (object)DBNull.Value),
                    new SqlParameter("@description", am.description ?? (object)DBNull.Value),
                    new SqlParameter("@profileImage", imagePath ?? (object)DBNull.Value),
                    new SqlParameter("@designation", am.designation ?? (object)DBNull.Value),
                    new SqlParameter("@experience", am.experience ?? (object)DBNull.Value)
                };
                Db.ExecuteNonQuery(insertQuery, insertParam);
                Db.UpdateTabcounter(counter + 1, "aboutMaster");
                TempData["Message"] = "About Details Saved Successfully.";
                return RedirectToAction("ShowAbout", "Admin");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;

                return View(am);
            }
        }
        [HttpGet]
        public ActionResult DeleteAbout(int id)
        {
            string query = @"UPDATE aboutMaster SET status = 10 WHERE id = @id";
            SqlParameter[] param =
            {
                new SqlParameter("@id", id)
            };
            Db.ExecuteNonQuery(query, param);
            TempData["Message"] = "About record deleted successfully";
            return RedirectToAction("ShowAbout", "Admin");
        }
        [HttpGet]
        public ActionResult ShowAbout()
        {
            string query = @"SELECT * FROM aboutMaster WHERE status != 10 ORDER BY id ASC";
            DataTable dt = Db.GetDataTable(query);
            List<aboutMaster> list = new List<aboutMaster>();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    aboutMaster item = new aboutMaster();
                    item.id = Convert.ToInt32(row["id"]);
                    item.heading = row["heading"].ToString();
                    item.description = row["description"].ToString();
                    item.profileImage = row["profileImage"].ToString();
                    item.designation = row["designation"] == DBNull.Value ? "" : row["designation"].ToString();
                    item.experience = row["experience"] == DBNull.Value ? "" : row["experience"].ToString();
                    item.status = row["status"] == DBNull.Value ? "" : row["status"].ToString().Trim();
                    list.Add(item);
                }
            }
            return View(list);
        }
        [HttpPost]
        public ActionResult ToggleAboutStatus(string id)
        {
            string sql = @"Update aboutMaster SET Status = CASE WHEN Status = '1' THEN '0' ELSE '1' END WHERE id = @Id";
            SqlParameter[] p =
            {
                new SqlParameter("@Id",id)
            };
            Db.ExecuteNonQuery(sql, p);
            TempData["Message"] = "Status updated successfully!";
            return RedirectToAction("ShowAbout", "Admin");
        }
        [HttpGet]
        public ActionResult ServiceMaster(string id)
        {
            string query = @"SELECT * FROM serviceMaster WHERE id = @id AND status != 10";
            serviceMaster sm = new serviceMaster();
            if (!string.IsNullOrEmpty(id))
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@id", id)
                };
                DataTable dt = Db.GetDataTable(query, param);
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    sm.id = Convert.ToInt32(row["id"]);
                    sm.serviceName = row["serviceName"] == DBNull.Value ? "" : row["serviceName"].ToString();
                    sm.description = row["description"] == DBNull.Value ? "" : row["description"].ToString();
                    sm.icon = row["icon"] == DBNull.Value ? "" : row["icon"].ToString();
                    sm.status = row["status"] == DBNull.Value ? "" : row["status"].ToString().Trim();
                }
            }
            return View(sm);
        }
        [HttpPost]
        public ActionResult ServiceMaster(serviceMaster sm)
        {
            if (sm.id > 0)
            {
                string updateQuery = @"UPDATE serviceMaster SET serviceName = @serviceName, description = @description, icon = @icon WHERE id = @id";
                SqlParameter[] updateParam =
                {
                        new SqlParameter("@id", sm.id),
                        new SqlParameter("@serviceName",sm.serviceName ?? (object)DBNull.Value),
                        new SqlParameter("@description", sm.description ?? (object)DBNull.Value),
                        new SqlParameter("@icon", sm.icon ?? (object)DBNull.Value)
                   };
                Db.ExecuteNonQuery(updateQuery, updateParam);
                TempData["Message"] = "Service Updated Successfully.";
                return RedirectToAction("ShowService", "Admin");
            }
            int counter = 1;
            try
            {
                object maxId = Db.ExecuteScalar("SELECT ISNULL(MAX(Id), 0) + 1 FROM serviceMaster");
                counter = Convert.ToInt32(maxId);
            }
            catch
            {
                counter = Db.TabCounter("serviceMaster");
            }
            string insertQuery = @"INSERT INTO serviceMaster(id, serviceName, description, icon, status)VALUES(@id, @serviceName, @description, @icon, 1)";
            SqlParameter[] insertParam =
            {
                new SqlParameter("id", counter),
                new SqlParameter("serviceName", sm.serviceName ?? (object)DBNull.Value),
                new SqlParameter("description", sm.description ?? (object)DBNull.Value),
                new SqlParameter("icon", sm.icon ?? (object)DBNull.Value)
            };
            Db.ExecuteNonQuery(insertQuery, insertParam);
            Db.UpdateTabcounter(counter + 1, "serviceMaster");
            TempData["Message"] = "Service Saved Successfully.";
            return RedirectToAction("ShowService", "Admin");
        }
        [HttpGet]
        public ActionResult DeleteService(int id)
        {
                string query = @"UPDATE serviceMaster SET status = 10 WHERE id = @id";
                SqlParameter[] param =
                {
                   new SqlParameter("@id", id)
                };
                Db.ExecuteNonQuery(query, param);
                TempData["Message"] = "Service deleted successfully.";
            return RedirectToAction("ShowService");
        }
        [HttpGet]
        public ActionResult ShowService()
        {
            string query = @"SELECT * FROM serviceMaster WHERE status != 10 ORDER BY id DESC";
            DataTable dt = Db.GetDataTable(query);
            List<serviceMaster> services = new List<serviceMaster>();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    serviceMaster sm = new serviceMaster();

                    sm.id = Convert.ToInt32(row["id"]);
                    sm.serviceName = row["serviceName"].ToString();
                    sm.icon = row["icon"].ToString();
                    sm.description = row["description"].ToString();
                    sm.status = row["status"].ToString();
                    
                    services.Add(sm);
                }
            }

            return View(services);
        }
        [HttpPost]
        public ActionResult ToggleServiceStatus(string id)
        {
            string sql = @"Update serviceMaster Set Status = CASE WHEN Status = '1' THEN '0' ELSE '1' END where id = @Id";
            SqlParameter[] p =
            {
                new SqlParameter("@Id", id)
            };
            Db.ExecuteNonQuery(sql, p);
            TempData["Message"] = "Status updated successfully.";
            return RedirectToAction("ShowService","Admin");
        }   
        [HttpGet]
        public ActionResult ShowContact()
        {
            string query = @"SELECT * FROM contactMaster WHERE status != 10 ORDER BY id DESC";
            DataTable dt = Db.GetDataTable(query);
            List<contactMaster> contacts = new List<contactMaster>();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    contactMaster cm = new contactMaster();
                    cm.id = Convert.ToInt32(row["id"]);
                    cm.name = row["name"].ToString();
                    cm.email = row["email"].ToString();
                    cm.phone = row["phone"].ToString();
                    cm.subject = row["subject"].ToString();
                    cm.message = row["message"].ToString();
                    if (row["date"] != DBNull.Value)
                    {
                        cm.date = Convert.ToDateTime(row["date"]);
                    }
                    cm.status = row["status"].ToString();
                    contacts.Add(cm);
                }
            }
            return View(contacts);
        }
        [HttpGet]
        public ActionResult DeleteContact(int id)
        {
            string query = @"Update contactMaster set status = 10 where id = @id";
            SqlParameter[] param =
            {
                new SqlParameter("@id", id)
            };
            Db.ExecuteNonQuery(query, param);
            TempData["Message"] = "Contact deleted successfully.";
            return RedirectToAction("ShowContact", "Admin");
        }
        [HttpPost]
        public ActionResult ToggleContactStatus(string id)
        {
            string sql = @"Update contactMaster set Status = CASE WHEN Status = '1' THEN '0' ELSE '1' END where id = @Id";
            SqlParameter[] p =
            {
                new SqlParameter("@Id", id)
            };
            Db.ExecuteNonQuery(sql, p);
            TempData["Message"] = "Status updated successfully.";
            return RedirectToAction("ShowContact", "Admin");
        }
        [HttpGet]
        public ActionResult SocialLink(string id)
        {
            socialLinkMaster model = new socialLinkMaster();

            if (!string.IsNullOrEmpty(id))
            {
                string query = @"SELECT * FROM socialLinkMaster WHERE id = @id AND status != 10";

                SqlParameter[] param =
                {
                   new SqlParameter("@id", id)
                };

                DataTable dt = Db.GetDataTable(query, param);

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];

                    model.id = Convert.ToInt32(row["id"]);
                    model.platformName = row["platformName"].ToString();
                    model.icon = row["icon"].ToString();
                    model.url = row["url"].ToString();
                    model.status = row["status"].ToString();
                }
            }

            return View("SocialLink", model);
        }
        [HttpPost]
        public ActionResult SocialLink(socialLinkMaster sl)
        {
            if (sl.id > 0)
            {
                string updateQuery = @"UPDATE socialLinkMaster SET platformName = @platformName, icon = @icon, url = @url WHERE id = @id";
                SqlParameter[] updateParam =
                {
                        new SqlParameter("@id", sl.id),
                        new SqlParameter("@platformName",sl.platformName ?? (object)DBNull.Value),
                        new SqlParameter("@icon", sl.icon ?? (object)DBNull.Value),
                        new SqlParameter("@url", sl.url ?? (object)DBNull.Value)
                   };
                Db.ExecuteNonQuery(updateQuery, updateParam);
                TempData["Message"] = "Social Links Updated Successfully.";
                return RedirectToAction("ShowLinks", "Admin");
            }
            int counter = 1;
            try
            {
                object maxId = Db.ExecuteScalar("SELECT ISNULL(MAX(Id), 0) + 1 FROM socialLinkMaster");
                counter = Convert.ToInt32(maxId);
            }
            catch
            {
                counter = Db.TabCounter("socialLinkMaster");
            }
            string insertQuery = @"INSERT INTO socialLinkMaster(id, platformName, icon, url, status)VALUES(@id, @platformName, @icon, @url, 1)";
            SqlParameter[] insertParam =
            {
                new SqlParameter("id", counter),
                new SqlParameter("platformName", sl.platformName ?? (object)DBNull.Value),
                new SqlParameter("icon", sl.icon ?? (object)DBNull.Value),
                new SqlParameter("url", sl.url ?? (object)DBNull.Value)
            };
            Db.ExecuteNonQuery(insertQuery, insertParam);
            Db.UpdateTabcounter(counter + 1, "socialLinkMaster");
            TempData["Message"] = "Social Link Saved Successfully.";
            return RedirectToAction("ShowLinks", "Admin");
        }
        [HttpGet]
        public ActionResult ShowLinks()
        {
            List<socialLinkMaster> list = new List<socialLinkMaster>();
            string query = @"SELECT * FROM socialLinkMaster WHERE status != 10 ORDER BY id DESC";
            DataTable dt = Db.GetDataTable(query, null);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    socialLinkMaster model = new socialLinkMaster();

                    model.id = Convert.ToInt32(row["id"]);
                    model.platformName = row["platformName"].ToString();
                    model.icon = row["icon"].ToString();
                    model.url = row["url"].ToString();
                    model.status = row["status"].ToString();

                    list.Add(model);
                }
            }

            return View("ShowLinks", "Admin");
        }
        [HttpGet]
        public ActionResult DeleteLink(int id)
        {
            string query = @"update socialLinkMaster set status = 10 where id = @id";
            SqlParameter[] param =
            {
                new SqlParameter("@id", id)
            };
            Db.ExecuteNonQuery(query, param);
            TempData["Message"] = "Social Links deleted successfully.";
            return RedirectToAction("ShowLinks", "Admin");
        }
        [HttpPost]
        public ActionResult ToggleLinkStatus(string id)
        {
            string sql = @"Update socialLinkMaster set Status = CASE WHEN Status = '1' THEN '0' ELSE '1' END where id = @Id";
            SqlParameter[] param =
            {
                new SqlParameter("@Id", id)
            };
            Db.ExecuteNonQuery(sql, param);
            TempData["Message"] = "Status updated successfully";
            return RedirectToAction("ShowLinks", "Admin");
        }
        [HttpGet]
        public ActionResult TestimonialMaster(string id)
        {
            testimonialMaster am = new testimonialMaster();
            if (!string.IsNullOrEmpty(id))
            {
                string query = @"SELECT * FROM testimonialMaster WHERE id = @id AND status != 10";
                SqlParameter[] param =
                {
                    new SqlParameter("@id", id)
                };
                DataTable dt = Db.GetDataTable(query, param);
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    am.id = Convert.ToInt32(row["id"]);
                    am.clientName = row["clientName"] == DBNull.Value ? "" : row["clientName"].ToString();
                    am.designation = row["designation"] == DBNull.Value ? "" : row["designation"].ToString();
                    am.companyName = row["companyName"] == DBNull.Value ? "" : row["companyName"].ToString();
                    am.clientImage = row["clientImage"] == DBNull.Value ? "" : row["clientImage"].ToString();
                    am.rating = Convert.ToInt32(row["rating"]);
                    am.displayOrder = Convert.ToInt32(row["displayOrder"]);
                    am.status = row["status"] == DBNull.Value ? "" : row["status"].ToString().Trim();
                }
            }
            return View(am);
        }
        [HttpPost]
        public ActionResult TestimonialMaster(testimonialMaster tm)
        {
            try
            {
                string imagePath = tm.clientImage;

                // IMAGE UPLOAD
                if (tm.imageFile != null && tm.imageFile.ContentLength > 0)
                {
                    string uploadDir = Server.MapPath("~/Content/Upload/Testimonial/");

                    if (!Directory.Exists(uploadDir))
                    {
                        Directory.CreateDirectory(uploadDir);
                    }

                    string extension = Path.GetExtension(tm.imageFile.FileName);

                    string uniqueFileName =
                        DateTime.Now.Ticks + extension;

                    string filePath =
                        Path.Combine(uploadDir, uniqueFileName);

                    tm.imageFile.SaveAs(filePath);

                    imagePath = "/Content/Upload/Testimonial/" + uniqueFileName;
                }

                // UPDATE
                if (tm.id > 0)
                {
                    string updateQuery = @"UPDATE testimonialMaster SET clientName = @clientName, designation = @designation, companyName = @companyName, message = @message, clientImage = @clientImage, rating = @rating, displayOrder = @displayOrder, status = @status
                WHERE id = @id";

                    SqlParameter[] updateParam =
                    {
                       new SqlParameter("@id", tm.id),
                       new SqlParameter("@clientName", tm.clientName ?? (object)DBNull.Value),
                       new SqlParameter("@designation", tm.designation ?? (object)DBNull.Value),
                       new SqlParameter("@companyName", tm.companyName ?? (object)DBNull.Value),
                       new SqlParameter("@message", tm.message ?? (object)DBNull.Value),
                       new SqlParameter("@clientImage", imagePath ?? (object)DBNull.Value),
                       new SqlParameter("@rating", tm.rating),
                       new SqlParameter("@displayOrder", tm.displayOrder),
                       new SqlParameter("@status", string.IsNullOrEmpty(tm.status) ? "1" : tm.status)
                    };

                    Db.ExecuteNonQuery(updateQuery, updateParam);
                    TempData["Message"] = "Testimonial details updated successfully.";
                    return RedirectToAction("showTestimonial", "Admin");
                }

                // INSERT
                int counter = Convert.ToInt32( Db.ExecuteScalar("SELECT ISNULL(MAX(id), 0) + 1 FROM testimonialMaster" ));

                string insertQuery = @"INSERT INTO testimonialMaster( id, clientName, designation, companyName, message, clientImage, rating, displayOrder, status) VALUES (@id, @clientName, @designation, @companyName,
                @message, @clientImage, @rating, @displayOrder, @status)";

                SqlParameter[] insertParam =
                {
                    new SqlParameter("@id", counter),
                    new SqlParameter("@clientName", tm.clientName ?? (object)DBNull.Value),
                    new SqlParameter("@designation", tm.designation ?? (object)DBNull.Value),
                    new SqlParameter("@companyName", tm.companyName ?? (object)DBNull.Value),
                    new SqlParameter("@message", tm.message ?? (object)DBNull.Value),
                    new SqlParameter("@clientImage", imagePath ?? (object)DBNull.Value),
                    new SqlParameter("@rating", tm.rating),
                    new SqlParameter("@displayOrder", tm.displayOrder),
                    new SqlParameter("@status", string.IsNullOrEmpty(tm.status) ? "1" : tm.status)
               };

                Db.ExecuteNonQuery(insertQuery, insertParam);
                Db.UpdateTabcounter(counter + 1, "testimonialMaster");
                TempData["Message"] = "Testimonial details saved successfully.";
                return RedirectToAction("showTestimonial", "Admin");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View("TestimonialMaster", tm);
            }
        }
        [HttpGet]
        public ActionResult showTestimonial()
        {
            List<testimonialMaster> list = new List<testimonialMaster>();
            string query = @"Select * from testimonialMaster where status != 10 ORDER BY displayOrder ASC, id DESC";
            DataTable dt = Db.GetDataTable(query, null);
            if(dt != null && dt.Rows.Count > 0)
            {
                foreach(DataRow row in dt.Rows)
                {
                    testimonialMaster model = new testimonialMaster();
                    model.id = Convert.ToInt32(row["id"]);
                    model.clientName = row["clientName"].ToString();
                    model.designation = row["designation"].ToString();
                    model.companyName = row["companyName"].ToString();
                    model.message = row["message"].ToString();
                    model.clientImage = row["clientImage"].ToString();
                    model.rating = Convert.ToInt32(row["rating"]);
                    model.displayOrder = Convert.ToInt32(row["displayOrder"]);
                    model.status = row["status"].ToString();
                    list.Add(model);
                }
            }
            return View(list);
        }
        [HttpGet]
        public ActionResult DeleteTestimonial(int id)
        {
            string query = @"Update testimonialMaster set status = 10 where id = @id";
            SqlParameter[] param =
            {
                new SqlParameter("@id", id)
            };
            Db.ExecuteNonQuery(query, param);
            TempData["Message"] = "testimonial deleted successfully.";
            return RedirectToAction("showTestimonial", "Admin");
        }
        [HttpPost]
        public ActionResult toggleTestimonialStatus(string id)
        {
            string query = @"Update testimonialMaster set status = CASE WHEN status = '1' THEN '0' ELSE '1' END WHERE id = @Id";
            SqlParameter[] param =
            {
                new SqlParameter("@Id", id)
            };
            Db.ExecuteNonQuery(query, param);
            TempData["Message"] = "Status Updated Successfully.";
            return RedirectToAction("showTestimonial", "Admin");
        }
        [HttpGet]
        public ActionResult SettingMaster(string id)
        {
            settingMaster sm = new settingMaster(); 
            if (string.IsNullOrEmpty(id)) 
            {
                sm.status = "1"; 
                return View(sm); 
            }
            string query = @"SELECT * FROM settingsMaster WHERE id = @id AND status != 10"; 
            SqlParameter[] param = 
            {
                new SqlParameter("@id", id) 
            };
            DataTable dt = Db.GetDataTable(query, param); 
            if (dt != null && dt.Rows.Count > 0) 
            {
                DataRow row = dt.Rows[0];
                sm.id = Convert.ToInt32(row["id"]);
                sm.siteName = row["siteName"].ToString();
                sm.email = row["email"].ToString();
                sm.phone = row["phone"].ToString();
                sm.address = row["address"].ToString();
                sm.logo = row["logo"].ToString();
                sm.favicon = row["favicon"].ToString();
                sm.resume = row["resume"].ToString();
                sm.footerText = row["footerText"].ToString();
                sm.copyrightText = row["copyrightText"].ToString();
                if (row["status"] != DBNull.Value) 
                    sm.status = row["status"].ToString();
            } 
            return View(sm); 
        }
        [HttpPost]
        public ActionResult SettingMaster(settingMaster sm, HttpPostedFileBase logoFile, HttpPostedFileBase faviconFile, HttpPostedFileBase resumeFile)
        {
            try
            {
                string logoPath = sm.logo;
                string faviconPath = sm.favicon;
                string resumePath = sm.resume;
                if (logoFile != null && logoFile.ContentLength > 0)
                {
                    string extension = Path.GetExtension(logoFile.FileName);
                    string fileName = "logo_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + extension; 
                    string folderPath = Server.MapPath("~/Content/Upload/Settings/");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    string fullPath = Path.Combine(folderPath, fileName); 
                    logoFile.SaveAs(fullPath); logoPath = "~/Content/Upload/Settings/" + fileName;
                }
                if (faviconFile != null && faviconFile.ContentLength > 0) 
                {
                    string extension = Path.GetExtension(faviconFile.FileName);
                    string fileName = "favicon_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + extension;
                    string folderPath = Server.MapPath("~/Content/Upload/Settings/");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    string fullPath = Path.Combine(folderPath, fileName);
                    faviconFile.SaveAs(fullPath); faviconPath = "~/Content/Upload/Settings/" + fileName;
                } 
                if (resumeFile != null && resumeFile.ContentLength > 0)
                {
                    string extension = Path.GetExtension(resumeFile.FileName);
                    string fileName = "resume_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + extension; 
                    string folderPath = Server.MapPath("~/Content/Upload/Settings/");
                    if (!Directory.Exists(folderPath)) 
                    {
                        Directory.CreateDirectory(folderPath); 
                    }
                    string fullPath = Path.Combine(folderPath, fileName); 
                    resumeFile.SaveAs(fullPath); resumePath = "~/Content/Upload/Settings/" + fileName; 
                }
                if (sm.id > 0) 
                {
                    string query = @" UPDATE settingsMaster SET siteName = @siteName, email = @email, phone = @phone, address = @address, logo = @logo, favicon = @favicon, resume = @resume, status = @status, footerText = @footerText, copyrightText = @copyrightText WHERE id = @id";
                    SqlParameter[] param = 
                   {
                         new SqlParameter("@id", sm.id),
                        new SqlParameter("@siteName", sm.siteName ?? ""),
                        new SqlParameter("@email", sm.email ?? ""),
                        new SqlParameter("@phone", sm.phone ?? ""),
                        new SqlParameter("@address", sm.address ?? ""),
                        new SqlParameter("@logo", logoPath ?? ""),
                        new SqlParameter("@favicon", faviconPath ?? ""),
                        new SqlParameter("@resume", resumePath ?? ""),
                        new SqlParameter("@status", sm.status), 
                        new SqlParameter("@footerText", sm.footerText ?? ""), 
                        new SqlParameter("@copyrightText", sm.copyrightText ?? "")   
                    };
                    Db.ExecuteNonQuery(query, param); 
                    TempData["Message"] = "Settings updated successfully.";
                    return RedirectToAction("ShowSettings"); 
                }
                int counter = 1;
                try
                {
                    object maxId = Db.ExecuteScalar("SELECT ISNULL(MAX(Id), 0) + 1 FROM settingsMaster");
                    counter = Convert.ToInt32(maxId);
                }
                catch
                {
                    counter = Db.TabCounter("settingsMaster");
                }
                string insertQuery = @" INSERT INTO settingsMaster (id, siteName, email, phone, address, logo, favicon, resume, status, footerText, copyrightText ) VALUES (@id, @siteName, @email, @phone, @address, @logo, @favicon, @resume, @status, @footerText, @copyrightText )";
                SqlParameter[] insertParam = 
               {
                    new SqlParameter("@id", counter),
                    new SqlParameter("@siteName", sm.siteName ?? ""), 
                    new SqlParameter("@email", sm.email ?? ""),
                    new SqlParameter("@phone", sm.phone ?? ""),
                    new SqlParameter("@address", sm.address ?? ""), 
                    new SqlParameter("@logo", logoPath ?? ""),
                    new SqlParameter("@favicon", faviconPath ?? ""),
                    new SqlParameter("@resume", resumePath ?? ""),
                    new SqlParameter("@status", sm.status),
                    new SqlParameter("@footerText", sm.footerText ?? ""),
                    new SqlParameter("@copyrightText", sm.copyrightText ?? "")
                };
                Db.ExecuteNonQuery(insertQuery, insertParam); 
                TempData["Message"] = "Settings saved successfully.";
                return RedirectToAction("ShowSettings", "Admin"); 
            } 
            catch (Exception ex) 
            {
                TempData["Error"] = "Something went wrong: " + ex.Message;
                return View(sm);
            }
        }
        [HttpGet]
        public ActionResult DeleteSettings(int id)
        {
            string query = @"Update settingsMaster set status = 10 where id = @id";
            SqlParameter[] param =
            {
                new SqlParameter("@id", id)
            };
            Db.ExecuteNonQuery(query, param);
            TempData["Message"] = "Settings deleted successfully.";
            return RedirectToAction("ShowSettings", "Admin");
        }
        [HttpGet]
        public ActionResult ShowSettings()
        {
            string query = @"SELECT * FROM settingsMaster WHERE status != 10 ORDER BY id DESC";
            DataTable dt = Db.GetDataTable(query);
            List<settingMaster> list = new List<settingMaster>();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    settingMaster sm = new settingMaster();

                    sm.id = Convert.ToInt32(row["id"]);
                    sm.siteName = row["siteName"].ToString();
                    sm.email = row["email"].ToString();
                    sm.phone = row["phone"].ToString();
                    sm.address = row["address"].ToString();
                    sm.logo = row["logo"].ToString();
                    sm.favicon = row["favicon"].ToString();
                    sm.resume = row["resume"].ToString();
                    sm.status = row["status"].ToString();
                    sm.footerText = row["footerText"].ToString();
                    sm.copyrightText = row["copyrightText"].ToString();

                    list.Add(sm);
                }
            }

            return View(list);
        }
        [HttpPost]
        public ActionResult ToggleSettingStatus(string id)
        {
            string query = @"Update settingsMaster set status = CASE WHEN status = '1' THEN '0' ELSE '1' END WHERE id = @Id";
            SqlParameter[] param =
            {
                new SqlParameter("@Id", id)
            };
            Db.ExecuteNonQuery(query, param);
            TempData["Message"] = "Status Updated Successfully.";
            return RedirectToAction("ShowSettings", "Admin");
        }
    }
}