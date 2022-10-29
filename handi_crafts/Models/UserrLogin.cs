using System;
using System.Collections.Generic;

#nullable disable

namespace handi_crafts.Models
{
    public partial class UserrLogin
    {
        public decimal Id { get; set; }
        public string UserrName { get; set; }
        public string Passwordd { get; set; }
        public decimal? RoleId { get; set; }
        public decimal? UserrId { get; set; }
        public string Email { get; set; }

        public virtual Role Role { get; set; }
        public virtual Userr Userr { get; set; }
    }
}
