﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiBasics.Models
{
    public class DtoUpdateProduct
    {

        public int Id { get; set; }
        public string ProductName { get; set; }

        public decimal? ProductPrice { get; set; }

        public int CategoryId { get; set; }

    }
}
