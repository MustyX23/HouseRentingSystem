namespace HouseRentingSystem.Services.Data
{
    using HouseRentingSystem.Data.Models;
    using HouseRentingSystem.Services.Data.Interfaces;
    using HouseRentingSystem.Web.Data;
    using HouseRentingSystem.Web.ViewModels.Home;
    using HouseRentingSystem.Web.ViewModels.House;
    using Microsoft.EntityFrameworkCore;

    public class HouseService : IHouseService
    {
        private HouseRentingDbContext dbContext;

        public HouseService(HouseRentingDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task CreateAsync(HouseFormModel model, string agentId)
        {
            House house = new House() 
            {
                Title = model.Title,
                Description = model.Description,
                Address = model.Address,
                ImageUrl = model.ImageUrl,
                PricePerMonth = model.PricePerMonth,
                AgentId = Guid.Parse(agentId),
                CategoryId = model.CategoryId
            };

            await dbContext.Houses.AddAsync(house);
            await dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<IndexViewModel>> LastThreeHousesAsync()
        {
            IEnumerable<IndexViewModel> lastThreeHouses = await dbContext
                .Houses
                .OrderByDescending(x => x.CreatedOn)
                .Take(3)
                .Select(h => new IndexViewModel()
                {
                    Id = h.Id.ToString(),
                    Title = h.Title,
                    ImageUrl = h.ImageUrl,
                    
                })                                
                .ToArrayAsync();

            return lastThreeHouses;
        }
    }
}
