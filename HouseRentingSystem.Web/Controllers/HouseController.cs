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

            try
            {
                IEnumerable<CategoryViewModel> categories
                = await categoryService.GetAllCategoriesAsync();

                HouseFormModel house = new HouseFormModel()
                {
                    Categories = categories,
                };

                return View(house);
            }
            catch (Exception)
            {
                return GeneralError();
            }
            
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
                TempData[SuccessMessage] = "You successfully added a house!";
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

        [AllowAnonymous]
        public async Task<IActionResult> Details(string id)
        {
            bool houseExists = await houseService.ExistsByIdAsync(id);           

            if (!houseExists)
            {
                TempData[ErrorMessage] = "That house doesn't exist at the moment";
                return RedirectToAction("All", "House");
            }

            try
            {
                HouseDetailsViewModel viewModel
                = await houseService.GetDetailsByIdAsync(id);

                return View(viewModel);
            }
            catch (Exception)
            {
                return GeneralError();
            }            
            
        }

        public async Task<IActionResult> Edit(string id)
        {
            string userId = User.GetId();
            bool isUserAgent = await agentService.AgentExistsByUserIdAsync(userId);

            bool houseExists = await houseService
                .ExistsByIdAsync(id);
            if (!houseExists)
            {
                TempData[ErrorMessage] = "House with the provided id does not exist!";

                return RedirectToAction("All", "House");

            }

            if (!isUserAgent)
            {
                TempData[ErrorMessage] = "Become an Agent if you want to edit a house info";
                return RedirectToAction("Become", "Agent");
            }

            string? agentId = await agentService.FindAgentIdByUserIdAsync(userId);

            bool isAgentOwner = await houseService
                .IsAgentWithIdOwnerOfHouseWithIdAsync(id, agentId!);

            if (!isAgentOwner)
            {
                TempData[ErrorMessage] = "Sorry, you are not an owner of the house";
                return RedirectToAction("Mine", "House");
            }

            try
            {
                HouseFormModel formModel = await houseService.GetHouseForEditByIdAsync(id);

                formModel.Categories = await categoryService.GetAllCategoriesAsync();

                return View(formModel);
            }
            catch (Exception)
            {
                return GeneralError();
            }           
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, HouseFormModel formModel)
        {
            if (!ModelState.IsValid)
            {
                formModel.Categories = await categoryService.GetAllCategoriesAsync();
                return View(formModel);
            }

            string userId = User.GetId();
            bool isUserAgent = await agentService.AgentExistsByUserIdAsync(userId);

            bool houseExists = await houseService
                .ExistsByIdAsync(id);
            if (!houseExists)
            {
                TempData[ErrorMessage] = "House with the provided id does not exist!";

                return RedirectToAction("All", "House");

            }

            if (!isUserAgent)
            {
                TempData[ErrorMessage] = "Become an Agent if you want to edit a house info";
                return RedirectToAction("Become", "Agent");
            }

            string? agentId = await agentService.FindAgentIdByUserIdAsync(userId);

            bool isAgentOwner = await houseService
                .IsAgentWithIdOwnerOfHouseWithIdAsync(id, agentId!);

            if (!isAgentOwner)
            {
                TempData[ErrorMessage] = "Sorry, you are not an owner of the house";
                return RedirectToAction("Mine", "House");
            }

            try
            {
                await houseService.EditHouseByIdAndFormModelAsync(id, formModel);
                TempData[SuccessMessage] = "You successfully edited the house!";
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Unexpected Error occured while trying editing your house :(");
                formModel.Categories = await categoryService.GetAllCategoriesAsync();
                return View(formModel);
            }

            return RedirectToAction("Details", "House", new {id});
        }

        public async Task<IActionResult> Delete(string id)
        {
            string userId = User.GetId();
            bool isUserAgent = await agentService.AgentExistsByUserIdAsync(userId);

            bool houseExists = await houseService
                .ExistsByIdAsync(id);
            if (!houseExists)
            {
                TempData[ErrorMessage] = "House with the provided id does not exist!";

                return RedirectToAction("All", "House");

            }

            if (!isUserAgent)
            {
                TempData[ErrorMessage] = "Become an Agent if you want to edit a house info";
                return RedirectToAction("Become", "Agent");
            }

            string? agentId = await agentService.FindAgentIdByUserIdAsync(userId);

            bool isAgentOwner = await houseService
                .IsAgentWithIdOwnerOfHouseWithIdAsync(id, agentId!);

            if (!isAgentOwner)
            {
                TempData[ErrorMessage] = "Sorry, you are not an owner of the house";
                return RedirectToAction("Mine", "House");
            }

            try
            {
               HousePreDeleteViewModel housePreDeleteViewModel 
                    = await houseService.GetHouseForDeleteByIdAsync(id);

                return View(housePreDeleteViewModel);
            }
            catch (Exception)
            {
                return GeneralError();
            }

        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id, HousePreDeleteViewModel viewModel)
        {
            string userId = User.GetId();
            bool isUserAgent = await agentService.AgentExistsByUserIdAsync(userId);

            bool houseExists = await houseService
                .ExistsByIdAsync(id);
            if (!houseExists)
            {
                TempData[ErrorMessage] = "House with the provided id does not exist!";

                return RedirectToAction("All", "House");

            }

            if (!isUserAgent)
            {
                TempData[ErrorMessage] = "Become an Agent if you want to edit a house info";
                return RedirectToAction("Become", "Agent");
            }

            string? agentId = await agentService.FindAgentIdByUserIdAsync(userId);

            bool isAgentOwner = await houseService
                .IsAgentWithIdOwnerOfHouseWithIdAsync(id, agentId!);

            if (!isAgentOwner)
            {
                TempData[ErrorMessage] = "Sorry, you are not an owner of the house";
                return RedirectToAction("Mine", "House");
            }

            try
            {
                await houseService.DeleteHouseByIdAsync(id);
                TempData[WarningMessage] = "Your house was successfuly deleted";
                return RedirectToAction("Mine", "House");
            }
            catch (Exception)
            {
                return GeneralError();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Rent(string id)
        {
            bool houseExists = await houseService.ExistsByIdAsync(id);

            bool isHouseRented = await houseService.IsRentedByIdAsync(id);

            bool isUserAgent = await agentService.AgentExistsByUserIdAsync(User.GetId());

            if (!houseExists)
            {
                TempData[ErrorMessage] = "House with provided id doesn't exist. Please try again.";
                return RedirectToAction("All", "House");
            }

            if (isHouseRented)
            {
                TempData[ErrorMessage] = "Selected house is already rented by someone else.";
                return RedirectToAction("All", "House");
            }

            if (isUserAgent)
            {
                TempData[ErrorMessage] = "As an agent you cannot rent houses. Please register as a user.";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                await houseService.RentHouseAsync(id, User.GetId());
                TempData[SuccessMessage] = "You successfully rented a house!";
            }
            catch (Exception)
            {
                return GeneralError();
            }

            return RedirectToAction("Mine", "House");

        }

        [HttpPost]
        public async Task<IActionResult> Leave(string id)
        {
            bool houseExists = await houseService.ExistsByIdAsync(id);

            bool isHouseRented = await houseService.IsRentedByIdAsync(id);

            bool isUserAgent = await agentService.AgentExistsByUserIdAsync(User.GetId());

            bool isCurrentUserRenterOfTheHouse = await houseService.IsRentedByUserWithRenterIdÁsync(id, User.GetId());

            if (!houseExists)
            {
                TempData[ErrorMessage] = "House with provided id doesn't exist. Please try again.";
                return RedirectToAction("All", "House");
            }

            if (!isHouseRented)
            {
                TempData[ErrorMessage] = "Selected house is not rented by you. Please select one of your rented houses.";
                return RedirectToAction("Mine", "House");
            }

            if (isUserAgent)
            {
                TempData[ErrorMessage] = "As an agent you cannot rent houses. Please register as a user.";
                return RedirectToAction("Index", "Home");
            }

            if (!isCurrentUserRenterOfTheHouse)
            {
                TempData[ErrorMessage] = "You must be the renter of the house in order to leave it.";
                return RedirectToAction("Mine", "House");
            }

            try
            {
                await houseService.LeaveHouseAsync(id);
                TempData[WarningMessage] = "You left the house.";
                return RedirectToAction("All", "House");
            }
            catch (Exception)
            {
                GeneralError();
            }

            return RedirectToAction("Mine", "House");
        }

        private IActionResult GeneralError()
        {
            TempData[ErrorMessage] = "Unexpected Error occured. Please try again later :(";
            return RedirectToAction("Home", "Index");
        }
    }
}
