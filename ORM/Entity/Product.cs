using System;
using System.Collections.Generic;

namespace WebApiBasics.ORM.Entity
{
    public partial class Product
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public decimal? ProductPrice { get; set; }
        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }
    }
}
