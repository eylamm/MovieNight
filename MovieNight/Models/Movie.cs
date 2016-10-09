using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MovieNight.Models
{
    public class Movie
    {
        [Key, Display(Name = "Movie ID")]
        public int ID { get; set; }

        [Required]
        public string Title { get; set; }

        [Required,
         Display(Name = "Release Date"),
         DataType(DataType.Date),
         Column(TypeName = "Date"),
         DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ReleaseDate { get; set; }

        [Required, Display(Description = "Enter genres separated with ;")]
        public string Genre { get; set; }

        [Required]
        public string Plot { get; set; }

        [Required]
        public int DirectorID { get; set; }

        [Required]
        public double Rating { get; set; }

        public string Poster { get; set; }

        public string Trailer { get; set; }

        [ForeignKey("DirectorID")]
        public virtual Director Director { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }
    }
}