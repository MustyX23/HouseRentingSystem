namespace HouseRentingSystem.Web.Infrastructure.Extensions
{
    using HouseRentingSystem.Web.ViewModels.Category.Interfaces;

    public static class ViewModelsExtensions
    {
        public static string GetUrlInformation(this ICategoryDetailsModel model)
        {
            return model.Name.Replace(" ", "-");
        }
    }
}
