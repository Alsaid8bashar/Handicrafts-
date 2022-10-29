using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace handi_crafts.Models
{
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public decimal Id { get; set; }
        [Display(Name = "Category Name")]

        public string CategoryName { get; set; }
        public string ImagePath { get; set; }
        [NotMapped]
        [Display(Name = "Image")]

        public IFormFile ImageFile { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
