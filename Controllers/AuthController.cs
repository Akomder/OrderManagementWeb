using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace OrderManagementWeb.Controllers
{
    public class AuthController : Controller
    {
        private OrderManagementDbEntities db = new OrderManagementDbEntities();

        // GET: Auth/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Username and password are required.";
                return View();
            }

            var user = db.Users.FirstOrDefault(u => u.UserName == username && u.Password == password);

            if (user != null)
            {
                if (user.Lock == true)
                {
                    ViewBag.Error = "This account is locked. Please contact administrator.";
                    return View();
                }

                // Set authentication cookie
                Session["UserID"] = user.UserID;
                Session["UserName"] = user.UserName;
                
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Error = "Invalid username or password.";
                return View();
            }
        }

        // GET: Auth/Logout
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login", "Auth");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
