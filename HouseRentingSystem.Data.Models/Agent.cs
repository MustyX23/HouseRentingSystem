namespace HouseRentingSystem.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using static HouseRentingSystem.Common.EntityValidationConstants.Agent;

    public class Agent
    {
        public Agent()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }

        [Required]
        [StringLength(PhoneNumberMaxLenght, MinimumLength =PhoneNumberMinLenght)]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        public Guid UserId { get; set; }

        public ApplicationUser User { get; set; } = null!;

        public ICollection<House> ManagedHouses { get; set; } = new HashSet<House>();
    }
}