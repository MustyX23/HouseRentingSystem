namespace HouseRentingSystem.Services.Data.Models.House
{
    using HouseRentingSystem.Web.ViewModels.House;

    public class AllHousesFilteredAndPagedServiceModel
    {
        public int TotalHousesCount { get; set; }

        public IEnumerable<HouseAllViewModel> Houses { get; set; } = new HashSet<HouseAllViewModel>();

    }
}
