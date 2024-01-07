namespace HouseRentingSystem.Services.Data
{
    using HouseRentingSystem.Data.Models;
    using HouseRentingSystem.Services.Data.Interfaces;
    using HouseRentingSystem.Services.Data.Models.House;
    using HouseRentingSystem.Web.Data;
    using HouseRentingSystem.Web.ViewModels.Home;
    using HouseRentingSystem.Web.ViewModels.House;
    using HouseRentingSystem.Web.ViewModels.House.Enums;
    using Microsoft.EntityFrameworkCore;

    public class HouseService : IHouseService
    {
        private HouseRentingDbContext dbContext;

        public HouseService(HouseRentingDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<AllHousesFilteredAndPagedServiceModel> AllAsync(AllHousesQueryModel queryModel)
        {
            IQueryable<House> housesQuery = dbContext
                .Houses
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryModel.Category))
            {
                housesQuery = housesQuery
                    .Where(h => h.Category.Name == queryModel.Category);
            }

            if (!string.IsNullOrWhiteSpace(queryModel.SearchString))
            {
                string wildCard = $"%{queryModel.SearchString.ToLower()}%";

                housesQuery = housesQuery
                    .Where(h => EF.Functions.Like(h.Title, wildCard)
                    || EF.Functions.Like(h.Address, wildCard) 
                    || EF.Functions.Like(h.Description, wildCard));
            }

            housesQuery = queryModel.HouseSorting switch
            {
                HouseSorting.Newest => housesQuery
                .OrderByDescending(h => h.CreatedOn),

                HouseSorting.Oldest => housesQuery
                .OrderBy(h => h.CreatedOn),

                HouseSorting.LowestPrice => housesQuery
                .OrderBy(h => h.PricePerMonth),

                HouseSorting.HighestPrice => housesQuery
                .OrderByDescending(h => h.PricePerMonth),

                HouseSorting.NotRentedFirst => housesQuery
                .OrderBy(h => h.RenterId != null)
                .ThenByDescending(h => h.Id),
                _ => housesQuery.OrderByDescending(h => h.Id),               
            };

            IEnumerable<HouseAllViewModel> allHouses = await housesQuery
                .Skip((queryModel.CurrentPage - 1) * queryModel.HousesPerPage)
                .Take(queryModel.HousesPerPage)
                .Select(h => new HouseAllViewModel()
                {
                    Id = h.Id.ToString(),
                    Address = h.Address,
                    ImageUrl = h.ImageUrl,
                    IsRented = h.RenterId.HasValue,
                    PricePerMonth = h.PricePerMonth,
                })
                .ToArrayAsync();

            int totalHouses = housesQuery.Count();

            return new AllHousesFilteredAndPagedServiceModel()
            {
                Houses = allHouses,
                TotalHousesCount = totalHouses,
            };
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
