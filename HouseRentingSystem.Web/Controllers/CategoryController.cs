using HouseRentingSystem.Services.Data.Interfaces;
using HouseRentingSystem.Web.ViewModels.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HouseRentingSystem.Web.Infrastructure.Extensions;

namespace HouseRentingSystem.Web.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ICategoryService categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        public async Task<IActionResult> All()
        {
            IEnumerable<AllCategoriesViewModel> viewModel =
                await categoryService.AllCategoriesForListAsync();

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id, string information)
        {
            bool categoryExists = await categoryService.ExistsByIdAsync(id);
            if (!categoryExists)
            {
                return NotFound();
            }

            CategoryDetailsViewModel viewModel =
                await categoryService.GetDetailsByIdAsync(id);

            if (viewModel.GetUrlInformation() != information)
            {
                return NotFound();
            }

            return View(viewModel);
        }
    }
}
