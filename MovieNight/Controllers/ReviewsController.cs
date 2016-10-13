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
    public class ReviewsController : Controller
    {
        private MovieNightDB db = new MovieNightDB();

        // GET: Reviews
        public ActionResult Index(string criticName, string reviewedMovie, string reviewContent, string groupByMovie)
        {
            /********************************************/
            /****** Search by Content or CriticName *****/
            /********************************************/

            // Create the reviews query
            var ReviewQry = from r in db.Reviews
                            select r;

            /*******************************/
            /******** Search by Movie ******/
            /*******************************/

            // Create the movies query - all movies with an id spcified in the review table
            var MoviesQry = from movie in db.Movies
                            join review in db.Reviews on movie.ID equals review.MovieID
                            select movie;

            // Create a new movie list
            var MovieLst = new List<string>();

            // Add only distinct genres to the genre list
            MovieLst.AddRange(MoviesQry.Select(movie => movie.Title).Distinct());

            // Initalize html helper with the list of movies that have been reviewed
            ViewBag.groupByMovie = new SelectList(MovieLst);

            /*********************************/
            /****** Apply Search Filters *****/
            /*********************************/

            // Check the search string wanted by the user
            if (!String.IsNullOrEmpty(criticName))
            {
                // Select all the review with the spciefied string in it's critic's name
                ReviewQry = ReviewQry.Where(review => review.CriticName.Contains(criticName));
            }

            // Check the search string wanted by the user
            if (!String.IsNullOrEmpty(reviewedMovie))
            {
                // Search for the movies with titles matching the search string
                MoviesQry = MoviesQry.Where(movie => movie.Title.Contains(reviewedMovie));

                // Get all reviews with the wanted movie ID
                ReviewQry = ReviewQry.Where(review => MoviesQry.Select(movie => movie.ID).Contains(review.MovieID));
            }

            // Check the search string wanted by the user
            if (!String.IsNullOrEmpty(reviewContent))
            {
                // Select all the reviews with the spciefied string in it's content
                ReviewQry = ReviewQry.Where(review => review.Content.Contains(reviewContent));
            }

            // Check the search string wanted by the user
            if (!String.IsNullOrEmpty(groupByMovie))
            {
                // Select the critics grouped by name
                var GroupbyRes = ReviewQry.GroupBy(r => r.Movie.Title).Select(lst => new { Name = lst.Key, Count = lst.Count() });
            }

            // Return the wanted reviews with the movie object to show - movie title
            return View(ReviewQry.Include(r => r.Movie));
        }

        // GET: Reviews/GroupBy
        public ActionResult GroupBy(string groupByMovie)
        {
            // Create the movies query - all movies with an id spcified in the review table
            var MoviesQry = from movie in db.Movies
                            join review in db.Reviews on movie.ID equals review.MovieID
                            select movie;

            // Create a new movie list
            var MovieLst = new List<string>();

            // Add only distinct genres to the genre list
            MovieLst.AddRange(MoviesQry.Select(movie => movie.Title).Distinct());

            // Initalize html helper with the list of movies that have been reviewed
            ViewBag.groupByMovie = new SelectList(MovieLst);

            var results = from p in db.Reviews
                          group p by p.Movie.Title into g
                          select new RenderView { MovieTitle = g.Key, ReviewCount = g.Count() };

            // If we wnat to group by all movies
            if (groupByMovie.Equals(""))
            {
                // Return all reviewed grouped by movie title
                return View(results);
            }

            // If the user wants to group by a specifiec movie
            else
            {
                // Select the specified movie 
                return View(results);
            }
        }

        // GET: Reviews/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        // GET: Reviews/Create
        public ActionResult Create()
        {
            ViewBag.MovieID = new SelectList(db.Movies, "ID", "Title");
            return View();
        }

        // POST: Reviews/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,CriticName,Date,MovieID,Content")] Review review, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                db.Reviews.Add(review);
                db.SaveChanges();
                if (String.IsNullOrEmpty(returnUrl))
                {
                    return RedirectToAction("Index");
                }

                return Redirect(returnUrl);
            }

            ViewBag.MovieID = new SelectList(db.Movies, "ID", "Title", review.MovieID);
            return View(review);
        }

        // GET: Reviews/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            ViewBag.MovieID = new SelectList(db.Movies, "ID", "Title", review.MovieID);
            return View(review);
        }

        // POST: Reviews/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,CriticName,Date,MovieID,Content")] Review review)
        {
            if (ModelState.IsValid)
            {
                db.Entry(review).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MovieID = new SelectList(db.Movies, "ID", "Title", review.MovieID);
            return View(review);
        }

        // GET: Reviews/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Review review = db.Reviews.Find(id);
            db.Reviews.Remove(review);
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
