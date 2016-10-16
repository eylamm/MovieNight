using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MovieNight.Models;
using TMDbLib.Client;
using TMDbLib.Objects.Search;

namespace MovieNight.Controllers
{
    public class DirectorsController : Controller
    {
        private MovieNightDB db = new MovieNightDB();
        TMDbClient client = new TMDbClient("e77a93ac7dab813a39327cfaa10938e8");

        // GET: Directors
        public ActionResult Index(string directorName, string directorGender, string directorOrigin)
        {
            // Set values received to keep the form withs its values
            ViewBag.directorName = directorName;
            ViewBag.selectedGender = directorGender;
            ViewBag.selectedOrigin = directorOrigin;

            /*****************************/
            /******* Search by Name ******/
            /*****************************/

            // Create the Directors query - all the directors in the db
            var DirectorsQry = from director in db.Directors
                               select director;

            /*******************************/
            /******* Search by Gender ******/
            /*******************************/

            // Initalize the viewbag with the possible genders
            ViewBag.directorGender = new SelectList(Enum.GetValues(typeof(Gender)));

            /********************************/
            /******* Search by Origing ******/
            /********************************/

            // Create a new genre list
            var OriginLst = new List<string>();

            // Add only distinct gendres to the gendre list
            OriginLst.AddRange(DirectorsQry.Select(director => director.Origin).Distinct());

            // Add the list to the viewbag object, so we can list it in the view - html dropdown list
            ViewBag.directorOrigin = new SelectList(OriginLst);

            /*********************************/
            /****** Apply Search Filters *****/
            /*********************************/

            // Check the search string wanted by the user
            if (!String.IsNullOrEmpty(directorName))
            {
                // Select all the directors with the specified string in their name
                DirectorsQry = DirectorsQry.Where(director => director.Name.Contains(directorName));
            }

            // Check the wanted gender to search by
            if (!string.IsNullOrEmpty(directorGender) && !directorGender.Equals("All"))
            {
                // Select all the directors of that gender
                DirectorsQry = DirectorsQry.Where(director => director.Gender.ToString().Equals(directorGender));
            }

            // Check the search string wanted by the user - and is not All
            if (!String.IsNullOrEmpty(directorOrigin) && !directorOrigin.Equals("All"))
            {
                // Select the directors from that origin
                DirectorsQry = DirectorsQry.Where(director => director.Origin.Equals(directorOrigin));
            }

            // Return the directors found
            return View(DirectorsQry.Include(s => s.Movies));
        }

        // GET: Directors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Director director = db.Directors.Find(id);
            if (director == null)
            {
                return HttpNotFound();
            }
            return View(director);
        }

        public ActionResult SearchApi(string query)
        {
            return this.Json(client.SearchPerson(query).Results);
        }

        public ActionResult GetDetailsFromApi(int id)
        {
            return this.Json(client.GetPerson(id));
        }

        // GET: Directors/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Directors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,FirstName,LastName,Gender,DateOfBirth,Origin,Picture")] Director director)
        {
            if (ModelState.IsValid)
            {
                db.Directors.Add(director);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(director);
        }

        // GET: Directors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Director director = db.Directors.Find(id);
            if (director == null)
            {
                return HttpNotFound();
            }
            return View(director);
        }

        // POST: Directors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,FirstName,LastName,Gender,DateOfBirth,Origin,Picture")] Director director)
        {
            if (ModelState.IsValid)
            {
                db.Entry(director).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(director);
        }

        // GET: Directors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Director director = db.Directors.Find(id);
            if (director == null)
            {
                return HttpNotFound();
            }
            return View(director);
        }

        // POST: Directors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Director director = db.Directors.Find(id);
            db.Directors.Remove(director);
            db.SaveChanges();
            return RedirectToAction("Index");
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