namespace HouseRentingSystem.Services.Data
{
    using HouseRentingSystem.Data.Models;
    using HouseRentingSystem.Services.Data.Interfaces;
    using HouseRentingSystem.Services.Data.Models.House;
    using HouseRentingSystem.Web.Data;
    using HouseRentingSystem.Web.ViewModels.Agent;
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
                .Where(h => h.IsActive)
                .Skip((queryModel.CurrentPage - 1) * queryModel.HousesPerPage)
                .Take(queryModel.HousesPerPage)
                .Select(h => new HouseAllViewModel()
                {
                    Id = h.Id.ToString(),
                    Title = h.Title,
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

        public async Task<IEnumerable<HouseAllViewModel>> AllByAgentIdAsync(string agentId)
        {
            IEnumerable<HouseAllViewModel> allAgentHouses = await dbContext
                .Houses
                .Where(h => h.IsActive && h.AgentId.ToString() == agentId)
                .Select(h => new HouseAllViewModel()
                {
                    Id = h.Id.ToString(),
                    Title = h.Title,
                    Address = h.Address,
                    IsActive = h.IsActive,
                    ImageUrl = h.ImageUrl,
                    IsRented = h.RenterId.HasValue,
                    PricePerMonth = h.PricePerMonth,
                }).ToArrayAsync();

            return allAgentHouses;
        }

        public async Task<IEnumerable<HouseAllViewModel>> AllByUserIdAsync(string userId)
        {
            IEnumerable<HouseAllViewModel> allUserHouses = await dbContext
                .Houses
                .Where(h => h.IsActive && h.RenterId.ToString() == userId 
                    && h.RenterId.HasValue)
                .Select(h => new HouseAllViewModel()
                {
                    Id = h.Id.ToString(),
                    Title = h.Title,
                    Address = h.Address,
                    ImageUrl = h.ImageUrl,
                    IsRented = h.RenterId.HasValue,
                    PricePerMonth = h.PricePerMonth,
                }).ToArrayAsync();

            return allUserHouses;
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

        public async Task DeleteHouseByIdAsync(string houseId)
        {
            House house = await dbContext.Houses
                .Where(h => h.IsActive)
                .FirstAsync(h => h.Id.ToString() == houseId);

            house.IsActive = false;

            await dbContext.SaveChangesAsync();
        }

        public async Task EditHouseByIdAndFormModelAsync(string houseId, HouseFormModel formModel)
        {
            House house = await dbContext.Houses
                .Where(h => h.IsActive)
                .FirstAsync(h => h.Id.ToString() == houseId);

            house.Title = formModel.Title;
            house.Description = formModel.Description;
            house.Address = formModel.Address;
            house.ImageUrl = formModel.ImageUrl;
            house.PricePerMonth = formModel.PricePerMonth;
            house.CategoryId = formModel.CategoryId;

            await dbContext.SaveChangesAsync();
        }

        public async Task<bool> ExistsByIdAsync(string id)
        {
            bool result = await dbContext.Houses
                .Where(h => h.IsActive)
                .AnyAsync(h => h.Id.ToString() == id);

            return result;
        }

        public async Task<HouseDetailsViewModel> GetDetailsByIdAsync(string houseId)
        {
            House house = await dbContext.Houses
                .Include(h => h.Category)
                .Include(h => h.Agent)
                .ThenInclude(h => h.User)
                .Where(h => h.IsActive)
                .FirstAsync(h => h.Id.ToString() == houseId);

            return new HouseDetailsViewModel()
            {
                Id = house!.Id.ToString(),
                Title = house.Title,
                Description = house.Description,
                Address = house.Address,
                Category = house.Category.Name,
                ImageUrl = house.ImageUrl,
                IsRented = house.RenterId.HasValue,
                PricePerMonth = house.PricePerMonth,
                Agent = new AgentInfoOnHouseViewModel()
                {
                    Email = house.Agent.User.Email,
                    PhoneNumber = house.Agent.PhoneNumber
                }
            };
        }

        public async Task<HousePreDeleteViewModel> GetHouseForDeleteByIdAsync(string houseId)
        {
            House house = await dbContext
                .Houses
                .Where(h => h.IsActive)
                .FirstAsync(h => h.Id.ToString() == houseId);

            return new HousePreDeleteViewModel()
            {
                Title = house.Title,
                Address = house.Address,
                ImageUrl = house.ImageUrl
            };
        }

        public async Task<HouseFormModel> GetHouseForEditByIdAsync(string houseId)
        {
            House house = await dbContext.Houses
                .Include(h => h.Category)
                .Where(h => h.IsActive)
                .FirstAsync(h => h.Id.ToString() == houseId);

            return new HouseFormModel() 
            {
                Title = house.Title,
                Description = house.Description,
                Address = house.Address,
                ImageUrl = house.ImageUrl,
                PricePerMonth = house.PricePerMonth,
                CategoryId = house.CategoryId,
            };
        }

        public async Task<bool> IsAgentWithIdOwnerOfHouseWithIdAsync(string houseId, string agentId)
        {
            House house = await dbContext
                .Houses
                .Where(h => h.IsActive)
                .FirstAsync(h => h.Id.ToString() == houseId);

            return house.AgentId.ToString() == agentId;
        }

        public async Task<IEnumerable<IndexViewModel>> LastThreeHousesAsync()
        {
            IEnumerable<IndexViewModel> lastThreeHouses = await dbContext
                .Houses
                .Where(h => h.IsActive)
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
