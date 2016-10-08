using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace MovieNight.Models
{
    public class Review
    {
        public int ID { get; set; }
        public string CriticName { get; set; }
        public DateTime ReviewDate { get; set; }
        public string MovieTitle { get; set; }
        public string ReviewContent { get; set; }
    }
    public class ReviewDBContext : DbContext
    {
        public DbSet<Review> Reviews { get; set; }
    }
}