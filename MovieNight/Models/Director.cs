using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MovieNight.Models
{
    public class Director
    {
        [Key, Display(Name = "Director ID")]
        public int ID { get; set; }

        [Required, Display(Name = "Name")]
        public string Name { get; set; }
        
        [Required, Display(Name = "Gender")]
        public Gender Gender { get; set; }

        [Required,
         Display(Name = "Birthday"),
         DataType(DataType.Date),
         Column(TypeName = "Date"),
         DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }

        [Required, Display(Name = "Origin")]
        public string Origin { get; set; }

        public string Picture { get; set; }

        public virtual ICollection<Movie> Movies { get; set; }
    }

    public enum Gender
    {
        Male,
        Female
    }
}