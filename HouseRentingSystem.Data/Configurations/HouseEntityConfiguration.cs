namespace HouseRentingSystem.Data.Configurations
{
    using Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using static HouseRentingSystem.Common.EntityValidationConstants;
    using House = Models.House;

    public class HouseEntityConfiguration : IEntityTypeConfiguration<House>
    {
        public void Configure(EntityTypeBuilder<House> builder)
        {
            builder
                .Property(h => h.CreatedOn)
                .HasDefaultValueSql("GETDATE()");

            builder
                .HasOne(h => h.Category)
                .WithMany(c => c.Houses)
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(h => h.Agent)
                .WithMany(a => a.ManagedHouses)
                .HasForeignKey(h => h.AgentId)
                .OnDelete(DeleteBehavior.Restrict);

            //Because of GDPR the renterer contains sensitive data a.k.a (Emails, PhoneNumbers, Passwords ect.)
            //And that is why the renterer's data and as a recor's
            //delete behaiviour must not be set as Restrict. 

            //builder
            //.HasOne(h => h.Renter)
            //.WithMany(r => r.RentedHouses)
            //.HasForeignKey(h => h.RenterId)
            //.OnDelete(DeleteBehavior.Restrict);

            builder.HasData(GenerateHouses());
        }

        private House[] GenerateHouses()
        {
            ICollection<House> houses = new HashSet<House>();

            House house;

            house = new House()
            {
                Title = "Big House Marina",
                Address = "North London, UK (near the border)",
                Description = "A big house for your whole family. Don't miss to buy a house with three bedrooms.",
                ImageUrl = "https://www.luxury-architecture.net/wpcontent/uploads/2017/12/1513217889-7597-FAIRWAYS-010.jpg",
                PricePerMonth = 2100.00M,
                CategoryId = 3,
                AgentId = Guid.Parse("6451E0AA-97CF-43BE-A42D-68F339C5E7EA"),
                RenterId = Guid.Parse("45e9217d-b923-4287-4b92-08dc0bef4f68")
            };

            houses.Add(house);

            house = new House()
            {
                Title = "Family House Comfort",
                Address = "Near the Sea Garden in Burgas, Bulgaria",
                Description = "It has the best comfort you will ever ask for. With two bedrooms, it is great for your family.",
                ImageUrl = "https://cf.bstatic.com/xdata/images/hotel/max1024x768/179489660.jpg?k=2029f6d9589b49c95dcc9503a265e292c2cdfcb5277487a0050397c3f8dd545a&o=&hp=1",
                PricePerMonth = 1200.00M,
                CategoryId = 2,
                AgentId = Guid.Parse("6451E0AA-97CF-43BE-A42D-68F339C5E7EA")
            };

            houses.Add(house);

            house = new House() 
            {
                Title = "Grand House",
                Address = "Boyana Neighbourhood, Sofia, Bulgaria",
                Description = "This luxurious house is everything you will need. It is just excellent.",
                ImageUrl = "https://i.pinimg.com/originals/a6/f5/85/a6f5850a77633c56e4e4ac4f867e3c00.jpg",
                PricePerMonth = 2000.00M,
                CategoryId = 3,
                AgentId = Guid.Parse("6451E0AA-97CF-43BE-A42D-68F339C5E7EA")
            };

            houses.Add(house);

            return houses.ToArray();
        }
    }
}
