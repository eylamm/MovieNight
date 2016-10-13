using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MovieNight.Models
{
    public class Review
    {
        [Key, Display(Name = "Review Number")]
        public int ID { get; set; }

        [Required, Display(Name = "Critic Name")]
        public string CriticName { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public int MovieID { get; set; }

        [Required]
        public string Content { get; set; }

        [ForeignKey("MovieID")]
        public virtual Movie Movie { get; set; }
    }

    public class RenderView
    {
        public string MovieTitle { get; set; }
        public int ReviewCount { get; set; }
    }
}