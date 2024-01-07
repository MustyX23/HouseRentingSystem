namespace HouseRentingSystem.Web.Controllers
{
    using static HouseRentingSystem.Common.NotificationMessagesConstants;

    using HouseRentingSystem.Services.Data.Interfaces;
    using HouseRentingSystem.Web.Infrastructure.Extensions;
    using HouseRentingSystem.Web.ViewModels.Category;
    using HouseRentingSystem.Web.ViewModels.House;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using HouseRentingSystem.Services.Data.Models.House;
    using NuGet.Packaging;

    [Authorize]
    public class HouseController : Controller
    {
        private readonly ICategoryService categoryService;
        private readonly IAgentService agentService;
        private readonly IHouseService houseService;

        public HouseController(ICategoryService categoryService, IAgentService agentService, IHouseService houseService)
        {
            this.categoryService = categoryService;
            this.agentService = agentService;
            this.houseService = houseService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> All([FromQuery]AllHousesQueryModel queryModel)
        {
            AllHousesFilteredAndPagedServiceModel serviceModel
                = await houseService.AllAsync(queryModel);

            queryModel.Houses = serviceModel.Houses;
            queryModel.TotalHouses = serviceModel.TotalHousesCount;
            queryModel.Categories = await categoryService.AllCategoryNamesAsync();

            return View(queryModel);
        }

        public async Task<IActionResult> Add()
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

        [HttpPost]
        public async Task<IActionResult> Add(HouseFormModel model)
        {
            string userId = User.GetId();
            bool isAgent = await agentService.AgentExistsByUserIdAsync(userId);

            if (!isAgent)
            {
                TempData[ErrorMessage] = "Become an Agent if you want to add houses";
                return RedirectToAction("Become", "Agent");
            }

            bool categoryExists = await categoryService
                .ExistsByIdAsync(model.CategoryId);

            if (!categoryExists)
            {
                //Adding model error automatically makes the ModelState invalid;
                ModelState.AddModelError(nameof(model.CategoryId), "Selected category doesn't exists");
            }

            if (!ModelState.IsValid)
            {
                model.Categories = await categoryService.GetAllCategoriesAsync();

                return View(model);
            }

            try
            {
                string? agentId 
                    = await agentService.FindAgentIdByUserIdAsync(userId);

                await houseService.CreateAsync(model, agentId!);
                return RedirectToAction("All", "House");
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Unexpected error occured while adding your new house. Please try again later or contact administrator");
                model.Categories = await categoryService.GetAllCategoriesAsync();

                return View(model);
            }
        }

        public async Task<IActionResult> Mine()
        {
            ICollection<HouseAllViewModel> myHouses 
                = new HashSet<HouseAllViewModel>();

            string userId = User.GetId();

            bool isAgent = await agentService.AgentExistsByUserIdAsync(userId);

            if (isAgent) 
            {
                string? agentId = await agentService.FindAgentIdByUserIdAsync(userId);

                myHouses.AddRange(await houseService.AllByAgentIdAsync(agentId!));
            }
            else
            {
                myHouses.AddRange(await houseService.AllByUserIdAsync(userId));
            }

            return View(myHouses);
        }
    }
}
