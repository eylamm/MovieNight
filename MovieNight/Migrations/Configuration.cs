namespace MovieNight.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using MovieNight.Models;
    using System.Collections.Generic;
    using TMDbLib.Client;
    using TMDbLib.Objects.General;
    using TMDbLib.Objects.Movies;
    using TMDbLib.Objects.Search;

    internal sealed class Configuration : DbMigrationsConfiguration<MovieNight.Models.MovieNightDB>
    {
        //  This method will be called after migrating to the latest version
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(MovieNight.Models.MovieNightDB context)
        {
            if (System.Diagnostics.Debugger.IsAttached == false)
            {
                System.Diagnostics.Debugger.Launch();
            }

            //  This method will be called after migrating to the latest version

            //var frank = new Director { Name = "Frank Darabont", DateOfBirth = DateTime.Parse("January 28, 1959"), Gender = Gender.Male, Origin = "France", Picture = "https://images-na.ssl-images-amazon.com/images/M/MV5BNjk0MTkxNzQwOF5BMl5BanBnXkFtZTcwODM5OTMwNA@@._V1_.jpg" };
            //var francis = new Director { Name = "Francis Ford Coppola", DateOfBirth = DateTime.Parse("April 7, 1939"), Gender = Gender.Male, Origin = "USA", Picture = "https://images-na.ssl-images-amazon.com/images/M/MV5BMTM5NDU3OTgyNV5BMl5BanBnXkFtZTcwMzQxODA0NA@@._V1_SY1000_CR0,0,665,1000_AL_.jpg" };
            //var christ = new Director { Name = "Christopher Nolan", DateOfBirth = DateTime.Parse("July 30, 1970"), Gender = Gender.Male, Origin = "UK", Picture = "https://images-na.ssl-images-amazon.com/images/M/MV5BNjE3NDQyOTYyMV5BMl5BanBnXkFtZTcwODcyODU2Mw@@._V1_.jpg" };

            //var directors = new List<Director>
            //{
            //    frank, francis, christ
            //};

//            directors.ForEach(s => context.Directors.AddOrUpdate(s));
//          context.SaveChanges();

            //var shawshank = new MovieNight.Models.Movie { Title = "The Shawshank Redemption", Genre = "Crime, Drama", Director = frank, Plot = "Two imprisoned men bond over a number of years, finding solace and eventual redemption through acts of common decency.", Poster = "https://images-na.ssl-images-amazon.com/images/M/MV5BODU4MjU4NjIwNl5BMl5BanBnXkFtZTgwMDU2MjEyMDE@._V1_SY1000_CR0,0,672,1000_AL_.jpg", Rating = 9.3, ReleaseDate = DateTime.Parse("September 23, 1994") };
            //var godfather = new MovieNight.Models.Movie { Title = "The Godfather", Genre = "Crime, Drama", Director = francis, Plot = "The aging patriarch of an organized crime dynasty transfers control of his clandestine empire to his reluctant son.", Poster = "https://images-na.ssl-images-amazon.com/images/M/MV5BMmE4MjdiZjctMjU3Ni00ZTM5LWIxNGMtNjgzZjQ2Y2Y3ZTliXkEyXkFqcGdeQXVyMjUzOTY1NTc@._V1_UX182_CR0,0,182,268_AL_.jpg", Rating = 9.2, ReleaseDate = DateTime.Parse("March 15, 1972") };
            //var godfather2 = new MovieNight.Models.Movie { Title = "The Godfather: Part II", Genre = "Crime, Drama", Director = francis, Plot = "The early life and career of Vito Corleone in 1920s New York is portrayed while his son, Michael, expands and tightens his grip on his crime syndicate stretching from Lake Tahoe, Nevada to pre-revolution 1958 Cuba.", Poster = "https://images-na.ssl-images-amazon.com/images/M/MV5BNDVjZjgxNTgtMGNhMC00YWU0LTg0YTQtNTkxNzBjMDBkNWYyXkEyXkFqcGdeQXVyNTA4NzY1MzY@._V1_UY268_CR3,0,182,268_AL_.jpg", Rating = 9.0, ReleaseDate = DateTime.Parse("December 12, 1974") };
            //var batman = new MovieNight.Models.Movie { Title = "The Dark Knight", Genre = "Action, Crime, Drama", Director = christ, Plot = "When the menace known as the Joker wreaks havoc and chaos on the people of Gotham, the caped crusader must come to terms with one of the greatest psychological tests of his ability to fight injustice.", Poster = "https://images-na.ssl-images-amazon.com/images/M/MV5BMTMxNTMwODM0NF5BMl5BanBnXkFtZTcwODAyMTk2Mw@@._V1_UX182_CR0,0,182,268_AL_.jpg", Rating = 9.0, ReleaseDate = DateTime.Parse("July 24, 2008") };

            //var movies = new List<MovieNight.Models.Movie>
            //{
            //    shawshank,
            //    godfather,
            //    godfather2,
            //    batman
            //};

//            movies.ForEach(s => context.Movies.AddOrUpdate(p => p.Title, s));
//          context.SaveChanges();

//            var rev1 = new Review { Movie = batman, Content = "Awsome movie, realy loved it", CriticName = "Eylam Milner", Date = DateTime.Now };
//            var rev2 = new Review { Movie = godfather, Content = "Awsome movie, realy loved it", CriticName = "Eylam Milner", Date = DateTime.Now };
//            var rev3 = new Review { Movie = batman, Content = "Great movie, I realy enjoyed it", CriticName = "Lucas Aides", Date = DateTime.Now };

//            var reviews = new List<Review>
//            {
//                rev1,rev2,rev3
//            };

//            reviews.ForEach(s => context.Reviews.AddOrUpdate(p => p.ID, s));
//            context.SaveChanges();

            var user = new User { Username = "user", Password = "user", Role = Role.SimpleUser };
            var admin = new User { Username = "admin", Password = "admin", Role = Role.Admin };
            context.Users.AddOrUpdate(p => p.ID, user);
            context.Users.AddOrUpdate(p => p.ID, admin);
            context.SaveChanges();

            // TMDB API
            // TMDB API
            // TMDB API
            // TMDB API

            // Connectiong to TMDB API
            TMDbClient client = new TMDbClient("e77a93ac7dab813a39327cfaa10938e8");

            // Get the top rated movies
            TMDbLib.Objects.General.SearchContainer<TMDbLib.Objects.General.MovieResult> TopRatedMovies = client.GetMovieList(MovieListType.TopRated);

            // Get the now playing movies
            TMDbLib.Objects.General.SearchContainer<TMDbLib.Objects.General.MovieResult> NowPlayingMovies = client.GetMovieList(MovieListType.NowPlaying);

            // Get the popular movies
            TMDbLib.Objects.General.SearchContainer<TMDbLib.Objects.General.MovieResult> PopularMovies = client.GetMovieList(MovieListType.Popular);

            // Get the upcoming movies
            TMDbLib.Objects.General.SearchContainer<TMDbLib.Objects.General.MovieResult> UpComingMovies = client.GetMovieList(MovieListType.Upcoming);

            string BaseImgURL = "https://image.tmdb.org/t/p/w300";

            var Addedmovies = new List<MovieNight.Models.Movie>();

            // Goes over all of the now playing movies
            foreach (MovieResult currMovie in NowPlayingMovies.Results)
            {
                // Get the genre list into one string 
                List<Genre> genreList = client.GetMovie(currMovie.Id).Genres;

                // The new string value for the genre list
                string currMoviegenre = "";

                // Goes over all of the genres and add them to the new string
                foreach (Genre cuurGenre in genreList)
                {
                    // Add the current genre to new genre string
                    currMoviegenre += cuurGenre.Name + ", ";
                }

                // Trim trailing comma and white space
                currMoviegenre = currMoviegenre.TrimEnd(' ').TrimEnd(',');

                // Set movie trailer base path
                string MovieTralierBasePath = "https://www.youtube.com/embed/";

                string currMovieTrailerPath = "";

                if (client.GetMovie(currMovie.Id, MovieMethods.Videos).Videos.Results.Count() != 0)
                {
                    // Get the current movie video path
                    currMovieTrailerPath = client.GetMovie(currMovie.Id, MovieMethods.Videos).Videos.Results[0].Key;
                }

                // Get the current movie's director ID
                TMDbLib.Objects.General.Crew currdirector = client.GetMovieCredits(currMovie.Id).Crew.Where(crew1 => crew1.Job == "Director").ElementAt(0);

                // Get the current director object
                TMDbLib.Objects.People.Person director = client.GetPerson(currdirector.Id);

                // Create the current movie director object
                Director NewDirector = new Director
                {
                    Name = director.Name,
                    Gender = Gender.Male,
                    DateOfBirth = (director.Birthday == null ? DateTime.Parse("January 1, 1900") : (DateTime)director.Birthday.Value.Date),
                    Origin = (director.PlaceOfBirth == null ? "Unknown" : director.PlaceOfBirth.Split(',').Last()),
                    Picture = BaseImgURL + director.ProfilePath
                };

                // Adds the new director to directors DB
                context.Directors.Add(NewDirector);
//              context.SaveChanges();

                // Create the new movie object
                var NewMovie = new MovieNight.Models.Movie
                {
                    Title       = currMovie.Title,
                    Genre       = currMoviegenre,
                    Director    = NewDirector,
                    Plot        = currMovie.Overview,
                    Poster      = BaseImgURL + currMovie.PosterPath,
                    Rating      = currMovie.VoteAverage,
                    ReleaseDate = currMovie.ReleaseDate.Value.Date,
                    Trailer     = MovieTralierBasePath + currMovieTrailerPath
                };

                // Adds the new movie to movies DB
                context.Movies.Add(NewMovie);
                context.SaveChanges();
            }
            // TMDB API
            // TMDB API
            // TMDB API
            // TMDB API
        }
    }
}
