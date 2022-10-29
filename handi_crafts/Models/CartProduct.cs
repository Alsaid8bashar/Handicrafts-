using System;
using System.Collections.Generic;

#nullable disable

namespace handi_crafts.Models
{
    public partial class CartProduct
    {
        public decimal Id2 { get; set; }
        public decimal? ProductId { get; set; }
        public decimal? CartId { get; set; }
        public decimal? Quantity { get; set; }

        public virtual Cart Cart { get; set; }
        public virtual Product Product { get; set; }
    }
}
