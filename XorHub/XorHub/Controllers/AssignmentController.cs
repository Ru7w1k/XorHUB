using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XorHub.Controllers
{
    public class AssignmentController : Controller
    {
        // GET: Assignment
        public ActionResult StudentResponse()
        {
            using (XorHubEntities db = new XorHubEntities())
            {
                List<SelectListItem> list = new List<SelectListItem>();
                foreach (var item in db.Batches)
                {
                    list.Add(new SelectListItem { Text = item.Name, Value = item.BatchId.ToString() });
                }

                ViewData["BatchList"] = list;
            }

            ViewBag.state = false;
            return View();
        }

        [HttpPost]
        public ActionResult PostQuestion(Assignment assignment)
        {
            assignment.PostedDate = DateTime.Now;
            assignment.TeacherName = Session["username"].ToString();

            using (XorHubEntities db = new XorHubEntities())
            {
                db.Assignments.Add(assignment);
                db.SaveChanges();
            }

            ViewBag.Message = "Question Successfully uploaded!";

            return RedirectToAction("Teacher", "Home");
        }
    }
}