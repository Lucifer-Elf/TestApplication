namespace Servize.Domain.Enums
{
    public class ServizeEnum
    {
        /* for free you can add only 2 cetogeries*/
        public enum Categories
        {
            CAR_CARE = 0,
            HOUSING = 1,
            PERSONAL = 2,
            HANDYMAN = 3,
            TUTOR = 4,
            APPLIANCE_REPAIR
        }
        public enum ContractType
        {
            MONTHLY = 0,
            FLEXIBLE = 1,
        }
        public enum ServizeModeType
        {
            FIXED = 0,
            FLEXIBLE = 1,
        }
        public enum PackageType
        {
            FREE = 0,
            BRONZE = 1,
            GOLD = 2,
        }

        public enum ServiceType
        {
            ST_HOMEVISIT,
            ST_ATSHOP
        }

        public enum Housing
        {
            CLEANING = 0,
            PAINTING_RENOVATION = 1,
            MOVER_PACKER = 2,
            MAIDSERVICE = 3,
            DISINFECTION = 4,
            ACSERVICEREPAIRE = 5,
        }
        public enum Automative
        {
            WASHING = 0,
            SERVICING = 1,
        }
        public enum Personal
        {
            GROMING = 0,
            SPA = 1,
            MANIANDPEDICURE = 2,
            PHOTOGRAPHER = 3,
            MAKEUP = 4,
            LAUNDRY = 5,
        }
        public enum HandyMan
        {

            HANDYMAN = 30,
            ELECTRICIAN = 31,
            CARPENTER = 32,
            PLUMBING = 33,
            ELECTRICALREPAIR = 34,

            T_ALLSUBJECT = 40,
        }

        public enum Area
        {
            UAE_DUBAI = 0,
            UAE_SHARJHA = 1,
            UAE_ABUDHABI = 2,
        }

        public enum BookingAssignment
        {
            BA_AUTOMATIC = 0,
            BA_MANUAL = 1
        }
    }
}


