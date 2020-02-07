using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Type should not be null or empty")]
        public string Type { get; set; }

        public ICollection<Vehicle> Vehicles { get; set; }
    }
}
