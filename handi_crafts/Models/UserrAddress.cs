using System;
using System.Collections.Generic;

#nullable disable

namespace handi_crafts.Models
{
    public partial class UserrAddress
    {
        public decimal Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public decimal? PhoneNumber { get; set; }
        public string ZipCode { get; set; }
        public decimal? UserrId { get; set; }

        public virtual Userr Userr { get; set; }
    }
}
