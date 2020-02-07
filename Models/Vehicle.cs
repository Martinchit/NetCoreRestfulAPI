using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class Vehicle
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title should not be null or empty")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description should not be null or empty")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price should not be null or empty")]
        public double Price { get; set; }

        [Required(ErrorMessage = "Model should not be null or empty")]
        public string Model { get; set; }

        [Required(ErrorMessage = "Engine should not be null or empty")]
        public string Engine { get; set; }

        [Required(ErrorMessage = "Color should not be null or empty")]
        public string Color { get; set; }

        [Required(ErrorMessage = "Company should not be null or empty")]
        public string Company { get; set; }

        [Required(ErrorMessage = "DatePosted should not be null or empty")]
        public DateTime DatePosted { get; set; }

        public bool IsHotAndNew { get; set; }

        public bool IsFeatured { get; set; }

        [Required(ErrorMessage = "Location should not be null or empty")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Condition should not be null or empty")]
        public string Condition { get; set; }

        [Required(ErrorMessage = "CategoryId should not be null or empty")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "UserId should not be null or empty")]
        public int UserId { get; set; }

        public ICollection<Image> Images { get; set; }
    }
}
