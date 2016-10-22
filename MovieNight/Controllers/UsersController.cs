using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MovieNight.Models;

namespace MovieNight.Controllers
{
    public class UsersController : Controller
    {
        private MovieNightDB db = new MovieNightDB();

        public ActionResult LogIn()
        {
            return View();
        }

        // POST: Users/Login
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogIn(string Username, string Password, string fromURI)
        {
            User DBuser = db.Users.Where(u => u.Username == Username).FirstOrDefault();

            if (DBuser == null)
            {
                ModelState.AddModelError("", "Username does not exists");
            }
            else if (DBuser.Password.Equals(Password))
            {
                Session["user"] = DBuser;
                if (fromURI != null)
                {
                    return Redirect(fromURI);
                }

                return RedirectToAction("Index", "Movies");
            }
            else
            {
                ModelState.AddModelError("", "Username and password do not match");
            }

            return View();
        }

        public ActionResult AccessDenied()
        {
            ModelState.AddModelError("", "Insuficient permissions. Please log in with an Admin user");
            return View("LogIn");
        }

        // POST: Users/Login
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AccessDenied(string Username, string Password)
        {
            User DBuser = db.Users.Where(u => u.Username == Username).FirstOrDefault();

            if (DBuser == null)
            {
                ModelState.AddModelError("", "Username does not exists");
            }
            else if (DBuser.Password.Equals(Password))
            {
                Session["user"] = DBuser;
                return RedirectToAction("Index", "Movies");
            }
            else
            {
                ModelState.AddModelError("", "Username and password do not match");
            }

            return View();
        }

        public ActionResult LogOut()
        {
            Session["user"] = null;
            return RedirectToAction("Index", "Movies");
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
