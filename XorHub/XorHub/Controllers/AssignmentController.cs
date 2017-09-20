using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using XorHub.Models;

namespace XorHub.Controllers
{
    public class AssignmentController : Controller
    {
        // GET: Assignment
        public ActionResult StudentResponse(int id)
        {
            AssignmentSolutionModel asModel = new AssignmentSolutionModel();
            asModel.Solution = new Solution();

            using (XorHubEntities db = new XorHubEntities())
            {
                List<SelectListItem> list = new List<SelectListItem>();
                foreach (var item in db.Batches)
                {
                    list.Add(new SelectListItem { Text = item.Name, Value = item.BatchId.ToString() });
                }

                asModel.Assignment = db.Assignments.Where(a => a.AssignmentId == id).FirstOrDefault();

                ViewData["BatchList"] = list;
            }
            ViewBag.filePath = "~/Database/Questions/" + id + ".pdf";
            ViewBag.state = false;
            return View(asModel);
        }

        [HttpPost]
        public ActionResult PostQuestion(HttpPostedFileBase QueFile, Assignment assignment)
        {
            assignment.PostedDate = DateTime.Now;
            assignment.TeacherName = Session["username"].ToString();

            using (XorHubEntities db = new XorHubEntities())
            {
                db.Assignments.Add(assignment);
                db.SaveChanges();
            }

            QueFile.SaveAs(Path.Combine(Server.MapPath("~/Database/Questions"), assignment.AssignmentId.ToString() + ".pdf"));

            ViewBag.Message = "Question Successfully uploaded!";

            return RedirectToAction("Teacher", "Home");
        }

        [HttpPost]
        public ActionResult Submit(HttpPostedFileBase solutionDoc, AssignmentSolutionModel asModel)
        {
            if (!Directory.Exists(Server.MapPath("~/Database/Solutions/" + Session["username"].ToString())))
            {
                Directory.CreateDirectory(Server.MapPath("~/Database/Solutions/" + Session["username"].ToString()));
            }

            solutionDoc.SaveAs(Path.Combine(Server.MapPath("~/Database/Solutions/" + Session["username"].ToString() + "/"), asModel.Assignment.AssignmentId + ".pdf"));
            
            return RedirectToRoute(new
                {
                    Controller = "Home",
                    Action = "Student",
                    id = 3
                }
            );
        }
    }
}