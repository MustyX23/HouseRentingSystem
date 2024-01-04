namespace HouseRentingSystem.Web.Controllers
{
    using HouseRentingSystem.Services.Data.Interfaces;
    using HouseRentingSystem.Web.Infrastructure.Extensions;
    using static HouseRentingSystem.Common.NotificationMessagesConstants;
    using HouseRentingSystem.Web.ViewModels.Category;
    using HouseRentingSystem.Web.ViewModels.House;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class HouseController : Controller
    {
        private readonly ICategoryService categoryService;
        private readonly IAgentService agentService;
        public HouseController(ICategoryService categoryService, IAgentService agentService)
        {
            this.categoryService = categoryService;
            this.agentService = agentService;

        }

        [AllowAnonymous]
        public async Task<IActionResult> All()
        {
            return View();
        }

        public async Task<IActionResult> Add(HouseFormModel model)
        {
            string userId = User.GetId();
            bool isAgent = await agentService.AgentExistsByUserIdAsync(userId);

            if (!isAgent)
            {
                TempData[ErrorMessage] = "Become an Agent if you want to add houses";
                return RedirectToAction("Become", "Agent");
            }

            IEnumerable<CategoryViewModel> categories
                = await categoryService.GetAllCategoriesAsync();

            HouseFormModel house = new HouseFormModel() 
            {
                Categories = categories,
            };

            return View(house);
        }
    }
}
