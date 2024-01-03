namespace HouseRentingSystem.Data.Models
{
    using Microsoft.AspNetCore.Identity;

    public class ApplicationUser : IdentityUser<Guid>
    {
        public ICollection<House> RentedHouses { get; set; } = new HashSet<House>();
    }
}
