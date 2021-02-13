using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servize.Domain.Enums
{
    public class ServizeEnum
    {
        // for free you can add only 2 cetogeries
        public enum Categories
        {
            AUTOMOTIVE = 0,
            HOUSING = 1,
            PERSONAL = 2,
            HANDYMAN = 3,
            TUTOR = 4,
        }
        public enum ServizeModeType
        {
            FIXED = 0,
            FLEXIBLE = 2,
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

        public enum SubCategories
        {

            H_CLEANING = 0,
            H_PAINTING = 1,
            H_MOVERANDPACKER = 2,
            H_MAIDSERVICE = 3,
            H_DISINFECTION = 4,
            H_ACSERVICEREPAIRE = 5,


            A_WASHING = 10,
            A_SERVICING = 11,

            P_GROMING = 20,
            P_SPA = 21,
            P_MANIANDPEDICURE = 22,
            P_PHOTOGRAPHER = 23,
            P_MAKEUP = 24,
            P_LAUNDRY = 25,

            HM_HANYMAN = 30,
            HM_ELECTRICIAN = 31,
            HM_CARPENTER = 32,
            HM_PLUMBING = 33,
            HM_ELECTRICALREPAIR = 34,

            T_ALLSUBJECT = 40,



        }


        public enum Area
        {
            UAE_DUBAI = 0,
            UAE_SHARJHA = 1,
            UAE_ABUDHABI = 2,
        }
    }
}

/*public enum HousingSubCategory
{
    H_CLEANING          = 0,
    H_PAINTING          = 1,
    H_MOVERANDPACKER    = 2,
    H_MAIDSERVICE       = 3,
}

public enum AUTOMOTIVE
{
    A_WASHING   = 0,
    A_SERVICING = 1,
}

public enum Personal
{
    P_SALON             = 0,
    P_SPA               = 1,
    P_MANIANDPEDICURE   = 2,
    P_PHOTOGRAPHER      = 3,


}
public enum HandyMan
{
    H_HANYMAN           = 0,
    H_ELECTRICIAN       = 1,
    H_CARPENTER         = 2,
    H_PLUMBING          = 3,
    H_ELECTRICALREPAIR  = 4,
}*/




