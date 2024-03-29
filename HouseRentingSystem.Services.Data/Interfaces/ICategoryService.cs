﻿using HouseRentingSystem.Web.ViewModels.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseRentingSystem.Services.Data.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryViewModel>> GetAllCategoriesAsync();

        Task<bool> ExistsByIdAsync(int id);

        Task<IEnumerable<string>> AllCategoryNamesAsync();

        Task<IEnumerable<AllCategoriesViewModel>> AllCategoriesForListAsync();

        Task<CategoryDetailsViewModel> GetDetailsByIdAsync(int id);
    }
}
