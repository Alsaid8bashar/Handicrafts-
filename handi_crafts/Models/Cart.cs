using System;
using System.Collections.Generic;

#nullable disable

namespace handi_crafts.Models
{
    public partial class Cart
    {
        public Cart()
        {
            CartProducts = new HashSet<CartProduct>();
        }

        public decimal Idd { get; set; }
        public decimal? UserId { get; set; }
        public DateTime? DatePurchase { get; set; }
        public byte? Purchasestate { get; set; }

        public virtual Userr User { get; set; }
        public virtual ICollection<CartProduct> CartProducts { get; set; }
    }
}
