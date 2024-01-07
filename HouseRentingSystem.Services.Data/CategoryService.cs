namespace HouseRentingSystem.Services.Data
{
    using HouseRentingSystem.Services.Data.Interfaces;
    using HouseRentingSystem.Web.Data;
    using HouseRentingSystem.Web.ViewModels.Category;
    using Microsoft.EntityFrameworkCore;

    public class CategoryService : ICategoryService
    {
        private readonly HouseRentingDbContext dbContext;
        public CategoryService(HouseRentingDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<string>> AllCategoryNamesAsync()
        {
            IEnumerable<string> allNames = await dbContext
                .Categories
                .Select(c => c.Name)
                .ToArrayAsync();

            return allNames;
        }

        public async Task<bool> ExistsByIdAsync(int id)
        {
            bool result = await dbContext
                .Categories.AnyAsync(c => c.Id == id);

            return result;
        }

        public async Task<IEnumerable<CategoryViewModel>> GetAllCategoriesAsync()
        {
            IEnumerable<CategoryViewModel> allSelectedCategories = await
                dbContext.Categories
                .AsNoTracking()
                .Select(c => new CategoryViewModel()
                {
                    Id = c.Id,
                    Name = c.Name,
                }).ToArrayAsync();

            return allSelectedCategories;
        }
    }
}
