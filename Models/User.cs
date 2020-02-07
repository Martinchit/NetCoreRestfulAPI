using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [Required(ErrorMessage = "Email should not be null or empty")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password should not be null or empty")]
        public string Password { get; set; }

        public string Phone { get; set; }

        public string ImageUrl { get; set; }

        public ICollection<Vehicle> Vehicles { get; set; }
    }
}
