using System;
using System.Collections.Generic;

#nullable disable

namespace handi_crafts.Models
{
    public partial class Content
    {
        public decimal Id { get; set; }
        public decimal? PageId { get; set; }
        public string Content1 { get; set; }
        public string Header { get; set; }

        public virtual Page Page { get; set; }
    }
}
