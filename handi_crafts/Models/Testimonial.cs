using System;
using System.Collections.Generic;

#nullable disable

namespace handi_crafts.Models
{
    public partial class Testimonial
    {
        public decimal Id { get; set; }
        public decimal? UserId { get; set; }
        public string Content { get; set; }
        public decimal? Status { get; set; }

        public virtual Userr User { get; set; }
    }
}
