
$(document).ready(function () {

    var orderBy = "";
    var numOfMoviesDisplay = 3;
    var postrBasePath = "https://image.tmdb.org/t/p/w300/7D6hM7IR0TbQmNvSZVtEiPM3H5h.jpg";

    $("#ShowMeMore").click(function () {
        $.post("/Movies/GetNextMovies",
        {
            numOfMovies: numOfMoviesDisplay,
            lastMovieTitle: $(".movieTitle").last().find("a").text(),
        },
        function (data, status) {
            console.log(data);
            console.log(status);

            // Parse the recieved data to JSON format
            var jsonMovies = JSON.parse(data)

            // Goes over all of the elements in the JSON data
            $.each(jsonMovies, function (key, data) {
                
                console.log("Object: " + key);
                console.log(jsonMovies[key]);

                // Copy the last movie element in the movies page
                var movieCopy = $(".movieElement").last().clone();

                // Set the new movie poster
                movieCopy.find("img").attr("src", jsonMovies[key].Poster);
                movieCopy.find("img").attr("alt", jsonMovies[key].Title);

                // Set the new movie title and title link
                movieCopy.find(".movieTitle a").text(jsonMovies[key].Title);
                movieCopy.find(".movieTitle a").attr("href", "/Movies/Details/" + jsonMovies[key].ID);

                // Set the new movie release date
                var movieYear = new Date(jsonMovies[key].ReleaseDate).getFullYear();
                movieCopy.find(".releaseYear").text(movieYear);
                
                // Clear the gneres of the copied movie
                movieCopy.find(".movieGenres").children().remove();

                // Split the genre array
                var afterSplit = jsonMovies[key].Genre.split(',');

                // Goes over all of the genres of the current movie
                for (var i = 0; i < afterSplit.length ; i++)
                {
                    // Get the cureent Gender of the movie
                    var currGenre = afterSplit[i];

                    // Create the button tag for the genre
                    var buttonTag = $("<button> </button>").attr("class", "btn btn-xs btn-default");
                    var linkTag   = $("<a> </a>").attr("href", "/?movieGenre=" + currGenre).text(currGenre);

                    // Append the link tag to the button tag
                    buttonTag.append(linkTag);

                    // Append them to the movieGenres class
                    movieCopy.find(".movieGenres").append(buttonTag);
                }

                // Set the new movie review count
                var reviewCount = $("<span> </span>").attr("class", "glyphicon glyphicon-comment");
                movieCopy.find(".reviewCount").text(jsonMovies[key].DirectorID).append(reviewCount);

                // Set the new movie rating
                var ratingStar = $("<span> </span>").attr("class", "glyphicon glyphicon-star");
                movieCopy.find(".ratingNumber").text(jsonMovies[key].Rating).append(ratingStar);

                // Appent the new movie element to the movies page
                $("#displayedMovies").append(movieCopy);
            });

            // Move the "Show Me More" button down below the new added movies
            $("#ShowMeMore-div").appendTo("#displayedMovies");

            // Check if there are more movies to display
            $.post("/Movies/GetNextMovies",
            {
                numOfMovies: numOfMoviesDisplay,
                lastMovieTitle: $(".movieTitle").last().find("a").text(),
            },

             function (data, status) {

                 // Parse the recieved data to JSON format
                 var jsonMovies = JSON.parse(data)

                 // If there are no more movies to display
                 if (jsonMovies.length == 0)
                 {
                     // Hide the Show Me More button
                     $("#ShowMeMore").hide()
                 }
             });
        });
    });
});