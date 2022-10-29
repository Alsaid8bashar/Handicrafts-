using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace handi_crafts.Models
{
    public partial class Image
    {
        public decimal Id { get; set; }
        public decimal? PageId { get; set; }
        public string ImagePath { get; set; }
       
        [NotMapped]
        public IFormFile ImageFile { get; set; }
        public virtual Page Page { get; set; }
    }
}
