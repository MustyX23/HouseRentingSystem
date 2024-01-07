﻿namespace HouseRentingSystem.Web.ViewModels.House
{
    using HouseRentingSystem.Web.ViewModels.House.Enums;
    using System.ComponentModel.DataAnnotations;
    using static HouseRentingSystem.Common.GeneralApplicationConstants;

    public class AllHousesQueryModel
    {
        public string? Category { get; set; }

        [Display(Name = "Search by text")]
        public string? SearchString { get; set; }

        [Display(Name = "Sort Houses By Name")]
        public HouseSorting HouseSorting { get; set; }

        public int CurrentPage { get; set; } = DefaultPage;

        [Display(Name = "Show Houses on Page")]
        public int HousesPerPage { get; set; } = EntitiesPerPage;

        public int TotalHouses { get; set; }

        public IEnumerable<string> Categories { get; set; } = new HashSet<string>();

        public IEnumerable<HouseAllViewModel> Houses { get; set; } = new HashSet<HouseAllViewModel>();
    }
}
