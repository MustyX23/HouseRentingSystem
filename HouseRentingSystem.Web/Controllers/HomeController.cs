namespace HouseRentingSystem.Web.Controllers
{
    using HouseRentingSystem.Services.Data.Interfaces;
    using HouseRentingSystem.Web.ViewModels.Home;
    using Microsoft.AspNetCore.Mvc;
    public class HomeController : Controller
    {
        private IHouseService houseService;
        public HomeController(IHouseService houseService)
        {
            this.houseService = houseService;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<IndexViewModel> lastThreeHouses = await houseService.LastThreeHousesAsync();

            return View(lastThreeHouses);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int statusCode)
        {
            if (statusCode == 404 || statusCode == 400)
            {
                return View("Error404");
            }
            return View();
        }
    }
}