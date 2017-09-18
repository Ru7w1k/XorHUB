using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XorHub.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Student()
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

            List<Assignment> assignmentList = new List<Assignment>();
            using (XorHubEntities db = new XorHubEntities())
            {
                var userName = Session["username"].ToString();
                var userBatchId = db.LoginInfoes.Where(u => u.Username.Equals(userName)).FirstOrDefault().BatchId;
                assignmentList = db.Assignments.Where(a => a.BatchId == userBatchId).ToList();
            }

            ViewBag.Assignments = assignmentList;

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
            using (XorHubEntities db = new XorHubEntities())
            {
                var userName = Session["username"].ToString();
                assignmentList = db.Assignments.Where(a => (a.BatchId == batchId) && (a.TeacherId.Equals(userName))).ToList();
            }
            return Json(assignmentList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Teacher()
        {
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
    }
}