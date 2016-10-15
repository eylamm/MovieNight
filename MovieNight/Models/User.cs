using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MovieNight.Models
{
    public class User
    {
        [Key, Display(Name = "User ID")]
        public int ID { get; set; }

        [Required, Display(Name = "UserName"), StringLength(100), Index("IX_UserName", IsUnique = true)]
        public string Username { get; set; }

        [Required, Display(Name = "Password"), DataType(DataType.Password)]
        public string Password { get; set; }
        
        [Required, Display(Name = "Role")]
        public Role Role { get; set; }
    }

    public enum Role
    {
        Admin,
        SimpleUser
    }
}