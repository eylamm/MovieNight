using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MovieNight.Models;
using System.Net.Http;
using System.Xml;
using Newtonsoft.Json;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;
using System.Web.Script.Serialization;

namespace MovieNight.Controllers
{
    public class MoviesController : Controller
    {
        private MovieNightDB db = new MovieNightDB();

        TMDbClient client = new TMDbClient("e77a93ac7dab813a39327cfaa10938e8");

        // GET: Movies
        public ActionResult Index(string searchString, string movieGenre, string searchDirector, string orderBy)
        {
            // Set values received to keep the form withs its values
            ViewBag.searchString = searchString;
            ViewBag.selectedGenre = movieGenre;
            ViewBag.searchDirector = searchDirector;
            ViewBag.orderBy = orderBy;

            /*******************************/
            /****** Search by Director *****/
            /*******************************/

            // Create the Directors query - all directors with an id spcified in the movies table
            var DirectorsQry = from director in db.Directors
                               join movie in db.Movies on director.ID equals movie.DirectorID
                               select director;

            /******************************/
            /******* Search by Genre ******/
            /******************************/

            // Create a new genre list
            var GenreLst = new List<string>();

            // Create a query to find all genres in the movies DB
            var GenreQry = from d in db.Movies
                           orderby d.Genre
                           select d.Genre;

            // Add only distinct genres to the genre list
            foreach (var genreRow in GenreQry)
            {
                foreach (var genre in genreRow.Split(','))
                {
                    if (!GenreLst.Contains(genre.Trim()))
                    {
                        GenreLst.Add(genre.Trim());
                    }
                }
            }

            // Add the list to the viewbag object, so we can list it in the view - html dropdown list
            ViewBag.movieGenre = new SelectList(GenreLst);

            /*******************************/
            /****** Search by Title ********/
            /*******************************/

            // Create the movie query
            var MoviesQuery = from m in db.Movies
                              select m;

            /*********************************/
            /****** Apply Search Filters *****/
            /*********************************/

            // Check the search string wanted by the user
            if (!String.IsNullOrEmpty(searchString))
            {
                // Order the movies by title
                MoviesQuery = MoviesQuery.OrderBy(m => m.Title);

                // Select all the movies with the spciefied string in it's title
                MoviesQuery = MoviesQuery.Where(s => s.Title.Contains(searchString));
            }

            // Check the wante movie genre to search
            if (!string.IsNullOrEmpty(movieGenre) && !movieGenre.Equals("All"))
            {
                // Select all movies with that genre
                MoviesQuery = MoviesQuery.Where(x => x.Genre.Contains(movieGenre));
            }

            // Check the search string wanted by the user
            if (!String.IsNullOrEmpty(searchDirector))
            {
                // Select the directors with the wanted string in their names
                DirectorsQry = DirectorsQry.Where(d => d.Name.Contains(searchDirector));

                // Select all the movies with the wanted director ID
                MoviesQuery = MoviesQuery.Where(movie => DirectorsQry.Select(director => director.ID).Contains(movie.DirectorID));
            }

            if (!String.IsNullOrEmpty(orderBy))
            {
                if (orderBy.Equals("TopRated"))
                {
                    MoviesQuery = MoviesQuery.OrderByDescending(movie => movie.Rating);
                }
                else if (orderBy.Equals("Newest"))
                {
                    MoviesQuery = MoviesQuery.OrderByDescending(movie => movie.ReleaseDate);
                }
            }

            // Return the movies after filtering, with the thier director data (forigen key)
            return View(MoviesQuery.Include(s => s.Director).Include(s => s.Reviews));
        }

        // GET: Movies
        public ActionResult Manage()
        {
            if (Session["user"] == null || (Session["user"] as User).Role != Role.Admin)
            {
                return RedirectToAction("AccessDenied", "Users");
            }

            return View(db.Movies.ToList());
        }

        public ActionResult SearchApi(string query)
        {
            if (Session["user"] == null || (Session["user"] as User).Role != Role.Admin)
            {
                return RedirectToAction("AccessDenied", "Users");
            }

            return this.Json(client.SearchMovie(query).Results);
        }

        public ActionResult GetDetailsFromApi(int id)
        {
            if (Session["user"] == null || (Session["user"] as User).Role != Role.Admin)
            {
                return RedirectToAction("AccessDenied", "Users");
            }
            TMDbLib.Objects.Movies.Movie result = client.GetMovie(id, MovieMethods.Videos);
            Crew Director = client.GetMovieCredits(id).Crew.Where(crew => crew.Job == "Director").FirstOrDefault();

            result.Homepage = "";
            result.ImdbId = Director.Name;
            result.VoteCount = Director.Id;
            Director dbDir = db.Directors.Where(d => d.Name.ToLower().Equals(Director.Name.ToLower())).FirstOrDefault();
            if (dbDir == null)
            {
                result.Homepage = "noDirector";
            }
            else
            {
                result.Homepage = dbDir.ID.ToString();
            }
            
            return this.Json(result);
        }

        public ActionResult GetNextMovies(int numOfMovies, string lastMovieTitle)
        {
            // Check the MovieNightDB state
            if (db.Database.Connection.State == ConnectionState.Open)
            {
                // Close any open DB connections
                db.Database.Connection.Close();
            }

            // Create the movie query
            var GetAllMovies = from m in db.Movies
                               select m;

            // Get the last movie displyed on page, from MovieNightDB
            MovieNight.Models.Movie lastMovie = GetAllMovies.Where(m => m.Title == lastMovieTitle).FirstOrDefault();

            // Get the next movies to return to page
            System.Collections.Generic.List<MovieNight.Models.Movie> NextMoviesToDisplay = new List<Models.Movie>();

            // Get the next movies to return to page
            NextMoviesToDisplay = GetAllMovies.Where(m => m.ID > lastMovie.ID).Take(numOfMovies).ToList();

            // Create a new movie list object
            List<MovieNight.Models.Movie> movieList = new List<Models.Movie>();

            // Create and initialize the movie object with the details of the next movie to display
            foreach (var currMovie in NextMoviesToDisplay)
            {
                // Create a new movie object
                MovieNight.Models.Movie MovieToAdd = new Models.Movie();

                // Set the new movie features
                MovieToAdd.ID          = currMovie.ID;
                MovieToAdd.Title       = currMovie.Title;
                MovieToAdd.Poster      = currMovie.Poster;
                MovieToAdd.ReleaseDate = currMovie.ReleaseDate;
                MovieToAdd.Genre       = currMovie.Genre;
                MovieToAdd.Rating      = currMovie.Rating;
                MovieToAdd.DirectorID  = currMovie.Reviews.Count();

                // Add the current movie to movies list
                movieList.Add(MovieToAdd);
            }

            // Return the wated movies to display, as JSON string
            return this.Json(JsonConvert.SerializeObject(movieList));
        }
        
        // GET: Movies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MovieNight.Models.Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }

            // Get the current movie id from TMDB
            var currMovieID = client.SearchMovie(movie.Title).Results[0].Id;

            // Get the current movie cast from TMDB
            var currMoviecast = client.GetMovieCredits(currMovieID).Cast;

            // Set the base image URL
            string BaseImageURL = "https://image.tmdb.org/t/p/w132_and_h132_bestv2";

            // Initalize html helper with cast info
            ViewBag.Cast = currMoviecast;

            // Initalize html helper with base image url
            ViewBag.baseImageURL = BaseImageURL;

            // Initalize html helper with TMDB ID of the movie
            ViewBag.currMovieTMDBID = currMovieID;

            // Get Morew movies like this one
            ViewBag.moreLikeThis = client.GetMovieSimilar(currMovieID);

            return View(movie);
        }

        // GET: Movies/Create
        public ActionResult Create()
        {
            if (Session["user"] == null || (Session["user"] as User).Role != Role.Admin)
            {
                return RedirectToAction("AccessDenied", "Users");
            }

            ViewBag.DirectorID = new SelectList(db.Directors, "ID", "Name");
            return View();
        }
        
        // POST: Movies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,ReleaseDate,Genre,Plot,DirectorID,Rating,Poster,Trailer")] MovieNight.Models.Movie movie)
        {
            if (Session["user"] == null || (Session["user"] as User).Role != Role.Admin)
            {
                return RedirectToAction("AccessDenied", "Users");
            }

            if (ModelState.IsValid)
            {
                db.Movies.Add(movie);
                db.SaveChanges();
                return RedirectToAction("Manage");
            }

            ViewBag.DirectorID = new SelectList(db.Directors, "ID", "Name", movie.DirectorID);
            return View(movie);
        }

        // GET: Movies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["user"] == null || (Session["user"] as User).Role != Role.Admin)
            {
                return RedirectToAction("AccessDenied", "Users");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MovieNight.Models.Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            ViewBag.DirectorID = new SelectList(db.Directors, "ID", "Name", movie.DirectorID);
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,ReleaseDate,Genre,Plot,DirectorID,Rating,Poster,Trailer")] MovieNight.Models.Movie movie)
        {
            if (Session["user"] == null || (Session["user"] as User).Role != Role.Admin)
            {
                return RedirectToAction("AccessDenied", "Users");
            }

            if (ModelState.IsValid)
            {
                db.Entry(movie).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Manage");
            }
            ViewBag.DirectorID = new SelectList(db.Directors, "ID", "Name", movie.DirectorID);
            return View(movie);
        }

        // GET: Movies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["user"] == null || (Session["user"] as User).Role != Role.Admin)
            {
                return RedirectToAction("AccessDenied", "Users");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MovieNight.Models.Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["user"] == null || (Session["user"] as User).Role != Role.Admin)
            {
                return RedirectToAction("AccessDenied", "Users");
            }

            MovieNight.Models.Movie movie = db.Movies.Find(id);
            db.Movies.Remove(movie);
            db.SaveChanges();
            return RedirectToAction("Manage");
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