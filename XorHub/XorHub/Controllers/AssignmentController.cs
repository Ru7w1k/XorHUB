using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using XorHub.Models;
using System.Data.Entity.Validation;

namespace XorHub.Controllers
{
    public class AssignmentController : Controller
    {
        // GET: Assignment
        public ActionResult StudentResponse(int id)
        {
            AssignmentSolutionModel asModel = new AssignmentSolutionModel();
            asModel.Solution = new Solution() { Stat = "P"};

            using (XorHubEntities db = new XorHubEntities())
            {
                List<SelectListItem> list = new List<SelectListItem>();
                foreach (var item in db.Batches)
                {
                    list.Add(new SelectListItem { Text = item.Name, Value = item.BatchId.ToString() });
                }

                asModel.Assignment = db.Assignments.Where(a => a.AssignmentId == id).FirstOrDefault();

                var sol = db.Solutions.Where(s => s.AssignmentId == id && s.Stat.Equals("A")).FirstOrDefault();
                if (sol != null)
                {
                    asModel.Solution = sol;
                }

                ViewData["BatchList"] = list;
            }
            ViewBag.filePath = "~/Database/Solutions/" + Session["username"].ToString() + "/" + id + ".pdf";
            ViewBag.state = false;

            return View(asModel);
        }

        public ActionResult ViewResponse(int id)
        {
            AssignmentSolutionModel asModel = new AssignmentSolutionModel();
            List<Solution> listSolutions = new List<Solution>();
            using (XorHubEntities db = new XorHubEntities())
            {
                asModel.Assignment = db.Assignments.Where(a => a.AssignmentId == id).FirstOrDefault();
                listSolutions = db.Solutions.Where(s => s.AssignmentId == id && s.Stat.Equals("A")).ToList();
            }

            ViewBag.solutions = listSolutions;

            return View(asModel);
        }

        public ActionResult TeacherResponse(int id)
        {
            AssignmentSolutionModel asModel = new AssignmentSolutionModel();
            List<Solution> listSolutions = new List<Solution>();
            using (XorHubEntities db = new XorHubEntities())
            {
                asModel.Assignment = db.Assignments.Where(a=>a.AssignmentId == id).FirstOrDefault();
                listSolutions = db.Solutions.Where(s => s.AssignmentId == id).ToList();
            }

            ViewBag.filePath = "~/Database/Questions/" + asModel.Assignment.AssignmentId + ".pdf";
            ViewBag.solutions = listSolutions;

            return View("TeacherResponse", asModel);
        }

        [HttpPost]
        public ActionResult PostQuestion(HttpPostedFileBase QueFile, Assignment assignment)
        {
            assignment.PostedDate = DateTime.Now;
            assignment.TeacherName = Session["username"].ToString();

 

            if (!Directory.Exists(Server.MapPath("~/Database/Questions/")))
            {
                Directory.CreateDirectory(Server.MapPath("~/Database/Questions/"));
            }

            try
            {
                QueFile.SaveAs(Path.Combine(Server.MapPath("~/Database/Questions"), assignment.AssignmentId.ToString() + ".pdf"));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Teacher", "Home", new { id = 1 });
            }

            using (XorHubEntities db = new XorHubEntities())
            {
                db.Assignments.Add(assignment);
                db.SaveChanges();
            }

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

            try
            {
                solutionDoc.SaveAs(Path.Combine(Server.MapPath("~/Database/Solutions/" + Session["username"].ToString() + "/"), asModel.Assignment.AssignmentId + ".pdf"));
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Please Select File!";
                ViewBag.filePath = "~/Database/Solutions/" + Session["username"].ToString() + "/" + asModel.Assignment.AssignmentId + ".pdf";
                return View("StudentResponse", asModel);
            }


            Solution soln = new Solution() {
                AssignmentId = asModel.Assignment.AssignmentId,
                Username = Session["username"].ToString(),
                Stat = "P",
                UploadedOn = DateTime.Now,
                Document = asModel.Assignment.AssignmentId + ".pdf",
                Comment = ""
            };
            using (XorHubEntities db = new XorHubEntities())
            {
                try
                {
                    db.Solutions.Add(soln);
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {

                }
            }

            return RedirectToRoute(new
            {
                Controller = "Home",
                Action = "Student",
                id = 3
            }
            );
        }

        public new ActionResult Response(int id)
        {
            Solution sol = new Solution();
            using (XorHubEntities db = new XorHubEntities())
            {
                sol = db.Solutions.Where(s => s.SolutionId == id).FirstOrDefault();
                sol.Assignment = db.Assignments.Where(a => a.AssignmentId == sol.AssignmentId).FirstOrDefault();
            }

            if (Session["usertype"].ToString().Equals("T"))
                ViewBag.IsDisplay = true;
            else
                ViewBag.IsDisplay = false;

            return View(sol);
        }


        [HttpParamAction]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Approve(Solution sol)
        {
            using (XorHubEntities db = new XorHubEntities())
            {
                var soln = db.Solutions.Where(s => s.SolutionId == sol.SolutionId).FirstOrDefault();
                soln.Stat = "A";
                soln.Comment = sol.Comment;
                db.SaveChanges();
            }
            return RedirectToAction("TeacherResponse", new { id = sol.AssignmentId });
        }

        [HttpParamAction]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Reject(Solution sol)
        {
            using (XorHubEntities db = new XorHubEntities())
            {
                var soln = db.Solutions.Where(s => s.SolutionId == sol.SolutionId).FirstOrDefault();
                soln.Stat = "R";
                soln.Comment = sol.Comment; db.SaveChanges();
            }

            return RedirectToAction("TeacherResponse", new { id = sol.AssignmentId });
        }


    }
}