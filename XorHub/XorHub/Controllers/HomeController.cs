using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XorHub.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Admin()
        {
            if (Session["username"] == null || Session["usertype"] == null)
            {
                return RedirectToRoute(new
                {
                    controller = "Index",
                    action = "Index",
                    id = 1
                });
            }

            switch (Session["usertype"].ToString())
            {
                case "T":
                    return RedirectToRoute(new
                    {
                        controller = "Home",
                        action = "Teacher",
                        id = 2
                    });

                case "S":
                    return RedirectToRoute(new
                    {
                        controller = "Home",
                        action = "Student",
                        id = 2
                    });

                case "A":
                default:
                    break;
            }
            return View();
        }

        public ActionResult Student(int? id)
        {
            if (Session["username"] == null || Session["usertype"] == null)
            {
                return RedirectToRoute(new
                {
                    controller = "Index",
                    action = "Index",
                    id = 1
                });
            }

            switch (Session["usertype"].ToString())
            {           
                case "T":
                    return RedirectToRoute(new
                    {
                        controller = "Home",
                        action = "Teacher",
                        id = 2
                    });

                case "A":
                    return RedirectToRoute(new
                    {
                        controller = "Home",
                        action = "Admin",
                        id = 2
                    });

                case "S":
                default:
                    break;
            }

            switch (id)
            {
                case 1:
                    ViewBag.Message = "Assignment Submitted Successfully!";
                    break;
            }

            List<Assignment> assignmentList = new List<Assignment>();
            List<Solution> solutionList = new List<Solution>();

            using (XorHubEntities db = new XorHubEntities())
            {
                var userName = Session["username"].ToString();
                var userBatchId = db.LoginInfoes.Where(u => u.Username.Equals(userName)).FirstOrDefault().BatchId;
                assignmentList = db.Assignments.Where(a => a.BatchId == userBatchId).ToList();

                solutionList = db.Solutions.Where(s => s.Username.Equals(userName)).ToList();
                foreach( Solution sol in solutionList)
                {
                    sol.Assignment = db.Assignments.Where(a => a.AssignmentId == sol.AssignmentId).ToList().FirstOrDefault();
                }

            }

            ViewBag.Assignments = assignmentList;
            ViewBag.Solutions = solutionList;

            return View();
        }

        public ActionResult GetAssignments()
        {
            List<Assignment> assignmentList = new List<Assignment>();
            using (XorHubEntities db = new XorHubEntities())
            {
                var userName = Session["username"].ToString();
                var userBatchId = db.LoginInfoes.Where(u => u.Username.Equals(userName)).FirstOrDefault().BatchId;
                assignmentList = db.Assignments.Where(a => a.BatchId == userBatchId).ToList();
            }
            return Json(assignmentList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAssignmentsForTeacher(int batchId)
        {
            List<Assignment> assignmentList = new List<Assignment>();
            List<Assignment> finalList = new List<Assignment>();

            using (XorHubEntities db = new XorHubEntities())
            {
                var userName = Session["username"].ToString();
                assignmentList = db.Assignments.Where(a => (a.BatchId == batchId) && (a.TeacherName.Equals(userName))).ToList();
            }

            foreach (var item in assignmentList)
            {
                finalList.Add(new Assignment {
                    AssignmentId = item.AssignmentId,
                    Title = item.Title,
                    TeacherName = item.TeacherName,
                    Deadline = item.Deadline
                });
            }

            return Json(finalList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Teacher(int? id)
        {
            if(id == 1)
            {
                ViewBag.Message = "Please Select File!";
            }

            if(id == 4)
            {
                ViewBag.Message = "Please Select Valid Date!";
            }

            if (Session["username"] == null || Session["usertype"] == null)
            {
                return RedirectToRoute(new
                {
                    controller = "Index",
                    action = "Index",
                    id = 1
                });
                //return RedirectToAction("Index", "Index", 1);
            }

            if (!Session["usertype"].Equals("T"))
            {
                return RedirectToRoute(new
                {
                    controller = "Index",
                    action = "Index",
                    id = 2
                });
                //return RedirectToAction("Index", "Index", 2);
            }

            using (XorHubEntities db = new XorHubEntities())
            {
                List<SelectListItem> list = new List<SelectListItem>();
                foreach (var item in db.Batches)
                {
                    list.Add(new SelectListItem { Text = item.Name, Value = item.BatchId.ToString() });
                }

                ViewData["BatchList"] = list;
            }
            return View();
        }
        

        public JsonResult GetTeacherRequests()
        {
            List<LoginInfo> listTeachers = new List<LoginInfo>();

            using (XorHubEntities db = new XorHubEntities())
            {
                foreach(var item in  db.LoginInfoes.Where(l => l.Usertype.Equals("T") && !l.Stat).ToList())
                {
                    listTeachers.Add(new LoginInfo { Name = item.Name, Username = item.Username });
                }
            }

            return Json(listTeachers, JsonRequestBehavior.AllowGet);
        }

        public void ApproveTeacher(string username)
        {
            using (XorHubEntities db = new XorHubEntities())
            {
                db.LoginInfoes.Where(l => l.Username.Equals(username)).FirstOrDefault().Stat = true;
                db.SaveChanges();
            }
        }


    }
}