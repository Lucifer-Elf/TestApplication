﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Servize.Domain.Enums.ServizeEnum;

namespace Servize.DTO.PROVIDER
{
    public class ServizeCategoryDTO
    {
        public ServizeCategoryDTO()
        {
                
        }

        public string ServiceName { get; set; }

        public SubCategories Category { get; set; }

        public string ImageList { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int DaysOFWork { get; set; }

        public double PriceCharge { get; set; }

        public double Discount { get; set; }

        public Area Areas { get; set; }
    }
}