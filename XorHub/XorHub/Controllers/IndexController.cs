using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XorHub.Controllers
{
    public class IndexController : Controller
    {
        // GET: Index
        public ActionResult Index(int? id)
        {
            if (id != null)
            {
                switch (id)
                {
                    case 1:
                        ViewBag.Message = "Login Required!";
                        break;

                    case 2:
                        ViewBag.Message = "Access Denied!";
                        break;

                    default:
                        ViewBag.Message = "";
                        break;
                }
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

        [HttpPost]
        public ActionResult Register(int BatchId, LoginInfo model)
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


            if (ModelState["Name"].Errors.Count > 0 || ModelState["Username"].Errors.Count > 0 || ModelState["Passwd"].Errors.Count > 0)
            {
                
                return View("Index", model);
            }

            if(model.Usertype.Equals("T"))
            {
                model.BatchId = null;
                model.Stat = false;
            }
            else
            {
                model.Stat = true;
            }

            using (XorHubEntities db = new XorHubEntities())
            {
                db.LoginInfoes.Add(model);
                db.SaveChanges();
            }

            ViewBag.Message = "User Registered Successfully!! ";
            ModelState.Clear();
            return View("Index");
        }

        public ActionResult Logout()
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

            Session["username"] = null;
            Session["usertype"] = null;
            ViewBag.Message = "Logged Out Successfully";
            return View("Index");
        }

        [HttpPost]
        public ActionResult Login(LoginInfo model)
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

            if (ModelState["Username"].Errors.Count > 0 || ModelState["Passwd"].Errors.Count > 0)
            {
                
                return View("Index", model);
            }

            using (XorHubEntities db = new XorHubEntities())
            {
                model = db.LoginInfoes.Where(u => u.Username.Equals(model.Username) && u.Passwd.Equals(model.Passwd)).FirstOrDefault();
                if(model == null)
                {
                    ViewBag.Message = "Invalid Username or password!!";
                    ModelState.Clear();
                    return View("Index");
                }
            }

            if (model.Usertype.Equals("T"))
            {
                if (!model.Stat)
                {
                    ViewBag.Message = "User Not Authorized! Contact Admin..";
                    return View("Index");
                }
                else
                {
                    Session["username"] = model.Username;
                    Session["usertype"] = model.Usertype;
                    ViewBag.username = model.Username;
                    return RedirectToAction("Teacher", "home");
                }
            }

            if(model.Usertype.Equals("A"))
            {
                Session["username"] = model.Username;
                Session["usertype"] = model.Usertype;
                ViewBag.username = model.Username;
                return RedirectToAction("Admin", "home");
            }

            Session["username"] = model.Username;
            Session["usertype"] = model.Usertype;
            ViewBag.username = model.Username;
            return RedirectToAction("Student", "home");
        }
    }
}