using System;
using System.Collections.Generic;

#nullable disable

namespace handi_crafts.Models
{
    public partial class Page
    {
        public Page()
        {
            Contents = new HashSet<Content>();
            Images = new HashSet<Image>();
        }

        public decimal Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Content> Contents { get; set; }
        public virtual ICollection<Image> Images { get; set; }
    }
}
