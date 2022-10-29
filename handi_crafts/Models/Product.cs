using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace handi_crafts.Models
{
    public partial class Product
    {
        public Product()
        {
            CartProducts = new HashSet<CartProduct>();
        }

        public decimal Id { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }
        public decimal? CategoryId { get; set; }
        [Display(Name = "Description")]

        public string Descriptionn { get; set; }
        public string Imagepath { get; set; }
        public decimal? Quantity { get; set; }
        [NotMapped]
        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<CartProduct> CartProducts { get; set; }
    }
}
