
$(document).ready(function () {

    var numOfMoviesDisplay = 3;
    var postrBasePath  = "https://image.tmdb.org/t/p/w300/7D6hM7IR0TbQmNvSZVtEiPM3H5h.jpg";


    $("#ShowMeMore").click(function () {
        $.post("/Movies/GetNextMovies",
        {
            numOfMovies: numOfMoviesDisplay,
            lastMovieTitle: $(".movieTitle").last().find("a").text()
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
                
                console.log(jsonMovies[key].Genre);

                var after = jsonMovies[key].Genre.split(',')
                console.log(after);

                for (var i = 0; i < jsonMovies[key].Genre.split(',').length ; i++)
                {
                    // Get the cureent Gender of the movie
                    var currGenre = after[i];

                    // Create the button tag for the genre
                    var buttonTag = $("<button> </button>").attr("class", "btn btn-xs btn-default");
                    var linkTag   = $("<a> </a>").attr("href", "/?movieGenre=" + currGenre).text(currGenre);

                    // Append the link tag to the button tag
                    buttonTag.append(linkTag);

                    // Append them to the movieGenres class
                    movieCopy.find(".movieGenres").append(buttonTag);
                }

                // Appent the new movie element to the movies page
                $("#displayedMovies").append(movieCopy);

                //// Create the specific movie div element
                //var movieDiv = $("<div></div>").attr("class", "col-sm-4 col-lg-4 col-md-4");

                //// Create the thumbnail div elemnt
                //var thumbnailDiv = $("<div></div>").attr("class", "thumbnail");

                //// Create the img tag
                //var imageTag = $("<img></img>").attr("src", jsonMovies[key].Poster)

                //// Create the caption div
                //var captionDiv = $("<div></div>").attr("class", "caption");
                
                //// Create the ratings div
                //var ratingDiv = $("<div></div>").attr("class", "ratings");

                //// Append the elemnts
                //thumbnailDiv.append(imageTag, captionDiv, ratingDiv);
                //movieDiv.append(thumbnailDiv);
                //$("#displayedMovies").append(movieDiv);
            });

            // Move the "Show Me More" button down below the new added movies
            $("#ShowMeMore-div").appendTo("#displayedMovies");
        });
    });
    
});