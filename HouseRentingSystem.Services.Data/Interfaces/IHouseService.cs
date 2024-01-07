﻿namespace HouseRentingSystem.Services.Data.Interfaces
{
    using HouseRentingSystem.Services.Data.Models.House;
    using HouseRentingSystem.Web.ViewModels.Home;
    using HouseRentingSystem.Web.ViewModels.House;
    

    public interface IHouseService
    {
        Task<IEnumerable<IndexViewModel>> LastThreeHousesAsync();

        Task CreateAsync(HouseFormModel model, string agentId);

        Task<AllHousesFilteredAndPagedServiceModel> AllAsync(AllHousesQueryModel queryModel);

        Task<IEnumerable<HouseAllViewModel>> AllByAgentIdAsync(string agentId);

        Task<IEnumerable<HouseAllViewModel>> AllByUserIdAsync(string userId);

        Task<bool> ExistsByIdAsync(string id);

        Task<HouseDetailsViewModel> GetDetailsByIdAsync(string houseId);

        Task<HouseFormModel> GetHouseForEditByIdAsync(string houseId);

        Task<bool> IsAgentWithIdOwnerOfHouseWithIdAsync(string houseId, string agentId);

        Task EditHouseByIdAndFormModelAsync(string houseId, HouseFormModel formModel);

        Task<HousePreDeleteViewModel> GetHouseForDeleteByIdAsync(string houseId);

        Task DeleteHouseByIdAsync(string houseId);

    }
}
