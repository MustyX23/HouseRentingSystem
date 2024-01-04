using HouseRentingSystem.Data.Models;
using HouseRentingSystem.Services.Data.Interfaces;
using HouseRentingSystem.Web.Data;
using HouseRentingSystem.Web.ViewModels.Agent;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseRentingSystem.Services.Data
{
    public class AgentService : IAgentService
    {
        private readonly HouseRentingDbContext dbContext;

        public AgentService(HouseRentingDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<bool> AgentExistsByPhoneNumberAsync(string phoneNumber)
        {
            bool result = await dbContext.Agents
                .AnyAsync(a => a.PhoneNumber == phoneNumber);

            return result;
        }

        public async Task<bool> AgentExistsByUserIdAsync(string userId)
        {
           bool result = await this.dbContext
                .Agents.AnyAsync(a => a.UserId.ToString() == userId);

            return result;
        }

        public async Task<bool> AgentHasRentsByUserIdAsync(string userId)
        {
            ApplicationUser? user = await dbContext
                .Users
                .FirstOrDefaultAsync(u => u.Id.ToString() == userId);

            if (user == null)
            {
                return false;
            }

            return user.RentedHouses.Any();
        }

        public async Task CreateAgentAsync(string userId, BecomeAgentFormModel model)
        {
            Agent agent = new Agent() 
            {
                PhoneNumber = model.PhoneNumber,
                UserId = Guid.Parse(userId)
            };

            await dbContext.Agents.AddAsync(agent);
            await dbContext.SaveChangesAsync();
        }

        public async Task<string?> FindAgentIdByUserIdAsync(string userId)
        {
            Agent? agent = await dbContext
                .Agents
                .FirstOrDefaultAsync(a => a.UserId.ToString() == userId);

            if (agent == null)
            {
                return null;
            }

            return agent.Id.ToString();
        }
    }
}
