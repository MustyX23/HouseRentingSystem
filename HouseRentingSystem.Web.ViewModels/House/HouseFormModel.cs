namespace HouseRentingSystem.Web.ViewModels.House
{
    using HouseRentingSystem.Web.ViewModels.Category;
    using System.ComponentModel.DataAnnotations;
    using static HouseRentingSystem.Common.EntityValidationConstants.House;

    public class HouseFormModel
    {
        [Required]
        [StringLength(TitleMaxLenght, MinimumLength = TitleMinLenght)]
        [Display(Name = "Title")]
        public string Title { get; set; } = null!;


        [Required]
        [StringLength(AddressMaxLenght, MinimumLength = AddressMinLenght)]
        [Display(Name = "Address")]
        public string Address { get; set; } = null!;

        [Required]
        [StringLength(DescriptionMaxLenght, MinimumLength = DescriptionMinLenght)]
        [Display(Name = "Description")]
        public string Description { get; set; } = null!;

        [Required]        
        [Display(Name = "ImageURL")]
        public string ImageUrl {  get; set; } = null!;

        [Range(typeof(decimal),PricePerMonthMin, PricePerMonthMax)]
        public decimal PricePerMonth {  get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        public IEnumerable<CategoryViewModel> Categories { get; set; } = new HashSet<CategoryViewModel>();
    }
}
