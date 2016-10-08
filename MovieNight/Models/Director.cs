using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace MovieNight.Models
{
    public class Director
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Origin { get; set; }
    }
    public class DirectorDBContext : DbContext
    {
        public DbSet<Director> Directors { get; set; }
    }
}