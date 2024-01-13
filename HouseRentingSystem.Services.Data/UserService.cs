using HouseRentingSystem.Data.Models;
using HouseRentingSystem.Services.Data.Interfaces;
using HouseRentingSystem.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace HouseRentingSystem.Services.Data
{
    public class UserService : IUserService
    {
        private readonly HouseRentingDbContext dbContext;

        public UserService(HouseRentingDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<bool> UserHasRentsAsync(string userId)
        {
            ApplicationUser user = await dbContext.Users
                .FirstAsync(u => u.Id.ToString() == userId);

            return user.RentedHouses.Any();
        }
    }
}
