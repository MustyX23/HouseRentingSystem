namespace HouseRentingSystem.Web.Controllers
{
    using HouseRentingSystem.Services.Data.Interfaces;
    using HouseRentingSystem.Web.Infrastructure.Extensions;
    using static HouseRentingSystem.Common.NotificationMessagesConstants;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using HouseRentingSystem.Web.ViewModels.Agent;

    [Authorize]
    public class AgentController : Controller
    {
        private IAgentService agentService;

        public AgentController(IAgentService agentService)
        {
            this.agentService = agentService;
        }

        public async Task<IActionResult> Become()
        {
            string userId = User.GetId();

            bool isAgent = await agentService.AgentExistsByUserIdAsync(userId);

            if (isAgent)
            {
                TempData[ErrorMessage] = "You are already an Agent!";
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Become(BecomeAgentFormModel model)
        {
            string userId = User.GetId();

            bool isAgent = await agentService.AgentExistsByUserIdAsync(userId);

            if (isAgent)
            {
                TempData[ErrorMessage] = "You are already an Agent!";
                return RedirectToAction("Index", "Home");
            }
            
            bool isPhoneNumberTaken =
                await agentService.AgentExistsByPhoneNumberAsync(model.PhoneNumber);

            if (isPhoneNumberTaken)
            {
                ModelState.AddModelError(nameof(model.PhoneNumber), "Agent with this phone number already exists.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool userHasActiveRents = 
                await agentService.AgentHasRentsByUserIdAsync(userId);

            if (userHasActiveRents)
            {
                TempData[ErrorMessage] = "Agents don't have any active rents.";

                return RedirectToAction("Mine", "House");
            }

            try
            {
               await agentService.CreateAgentAsync(userId, model);
            }
            catch (Exception)
            {
                TempData[ErrorMessage] = "Unexpected Error occured while registering you as an Agent! Please try again later, or contact an administrator.";
                RedirectToAction("Index", "Home");
            }

            return RedirectToAction("All", "House");
           
        }
    }
}
