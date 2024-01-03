using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseRentingSystem.Common
{
    public static class EntityValidationConstants
    {
        public static class Category
        {
            public const int NameMinLenght = 2;
            public const int NameMaxLenght = 50;
        }

        public static class House
        {
            public const int TitleMinLenght = 10;
            public const int TitleMaxLenght = 50;

            public const int AddressMinLenght = 30;
            public const int AddressMaxLenght = 150;

            public const int DescriptionMinLenght = 50;
            public const int DescriptionMaxLenght = 500;

            public const string PricePerMonthMin = "0";
            public const string PricePerMonthMax = "2000";
        }

        public static class Agent
        {
            public const int PhoneNumberMinLenght = 7;
            public const int PhoneNumberMaxLenght = 15;
        }
    }
}
