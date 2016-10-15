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

            var video = client.GetMovie(123).Video;
            var videolist = client.GetMovieVideos(3386);

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
                DirectorsQry = DirectorsQry.Where(d => d.FirstName.Contains(searchDirector) || d.LastName.Contains(searchDirector));

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

            return View(movie);
        }

        // GET: Movies/Create
        public ActionResult Create()
        {
            ViewBag.DirectorID = new SelectList(db.Directors, "ID", "FirstName");
            return View();
        }

        public async System.Threading.Tasks.Task<ActionResult> Search(string title)
        {
            string _address = "http://www.omdbapi.com/?s=" + title + "&type=movie&r=xml";
            try
            {
                ViewBag.Movies = new List<string>();
                HttpWebRequest request = WebRequest.Create(_address) as HttpWebRequest;
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(response.GetResponseStream());

                XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                nsmgr.AddNamespace("rest", "http://schemas.microsoft.com/search/local/ws/rest/v1");

                //Get all locations in the response and then extract the formatted address for each location
                XmlNodeList moviesElement = xmlDoc.SelectNodes("//rest:result", nsmgr);
                foreach (XmlNode movie in moviesElement)
                {
                    ViewBag.Movies.Add(movie.SelectSingleNode(".//rest:title", nsmgr).InnerText);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                Console.Read();
                return null;
            }
            
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,ReleaseDate,Genre,Plot,DirectorID,Rating,Poster,Trailer")] MovieNight.Models.Movie movie)
        {
            if (ModelState.IsValid)
            {
                db.Movies.Add(movie);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DirectorID = new SelectList(db.Directors, "ID", "FirstName", movie.DirectorID);
            return View(movie);
        }

        // GET: Movies/Edit/5
        public ActionResult Edit(int? id)
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
            ViewBag.DirectorID = new SelectList(db.Directors, "ID", "FirstName", movie.DirectorID);
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,ReleaseDate,Genre,Plot,DirectorID,Rating,Poster,Trailer")] MovieNight.Models.Movie movie)
        {
            if (ModelState.IsValid)
            {
                db.Entry(movie).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DirectorID = new SelectList(db.Directors, "ID", "FirstName", movie.DirectorID);
            return View(movie);
        }

        // GET: Movies/Delete/5
        public ActionResult Delete(int? id)
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
            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MovieNight.Models.Movie movie = db.Movies.Find(id);
            db.Movies.Remove(movie);
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