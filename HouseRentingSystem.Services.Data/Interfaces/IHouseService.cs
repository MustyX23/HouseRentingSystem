namespace HouseRentingSystem.Services.Data.Interfaces
{
    using HouseRentingSystem.Services.Data.Models.House;
    using HouseRentingSystem.Web.ViewModels.Home;
    using HouseRentingSystem.Web.ViewModels.House;
    

    public interface IHouseService
    {
        Task<IEnumerable<IndexViewModel>> LastThreeHousesAsync();

        Task CreateAsync(HouseFormModel model, string agentId);

        Task<AllHousesFilteredAndPagedServiceModel> AllAsync(AllHousesQueryModel queryModel);
    }
}
