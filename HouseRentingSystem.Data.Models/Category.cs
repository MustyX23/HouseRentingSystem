namespace HouseRentingSystem.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using static HouseRentingSystem.Common.EntityValidationConstants.Category;

    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(NameMaxLenght, MinimumLength = NameMinLenght)]
        public string Name { get; set; } = null!;

        public IEnumerable<House> Houses { get; set; } = new HashSet<House>();
    }
}
