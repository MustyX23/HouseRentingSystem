using HouseRentingSystem.Data.Models;
using HouseRentingSystem.Web.ViewModels.Agent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseRentingSystem.Services.Data.Interfaces
{
    public interface IAgentService
    {
        Task<bool>AgentExistsByUserIdAsync(string userId);

        Task<bool>AgentExistsByPhoneNumberAsync(string phoneNumber);

        Task<bool> AgentHasRentsByUserIdAsync(string userId);

        Task CreateAgentAsync(string userId, BecomeAgentFormModel model);

        Task<string?> FindAgentIdByUserIdAsync(string userId);

        Task<bool> HasHouseWithIdAsync(string houseId, string agentId);
    }
}
