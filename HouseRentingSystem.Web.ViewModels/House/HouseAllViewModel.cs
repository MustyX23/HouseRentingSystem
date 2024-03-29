﻿using System.ComponentModel.DataAnnotations;

namespace HouseRentingSystem.Web.ViewModels.House
{
    public class HouseAllViewModel
    {
        public string Id { get; set; } = null!;

        public string Title { get; set; } = null!;

        public string Address { get; set; } = null!;

        [Display(Name = "ImageURL")]
        public string ImageUrl { get; set; } = null!;

        public bool IsActive { get; set; }

        [Display(Name = "Monthly Price")]
        public decimal PricePerMonth { get; set; }

        [Display(Name = "Is Rented?")]
        public bool IsRented { get; set; }
    }
}
