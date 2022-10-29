using System;
using System.Collections.Generic;

#nullable disable

namespace handi_crafts.Models
{
    public partial class Role
    {
        public Role()
        {
            UserrLogins = new HashSet<UserrLogin>();
        }

        public decimal Id { get; set; }
        public string RoleName { get; set; }

        public virtual ICollection<UserrLogin> UserrLogins { get; set; }
    }
}
