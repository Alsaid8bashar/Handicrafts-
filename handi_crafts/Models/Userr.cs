using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace handi_crafts.Models
{
    public partial class Userr
    {
        public Userr()
        {
            Carts = new HashSet<Cart>();
            PaymentMethods = new HashSet<PaymentMethod>();
            Testimonials = new HashSet<Testimonial>();
            UserrAddresses = new HashSet<UserrAddress>();
            UserrLogins = new HashSet<UserrLogin>();
        }

        public decimal Id { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public string ImagePath { get; set; }
        [NotMapped]
        [Display(Name = "Image")]
      
        public IFormFile ImageFile { get; set; }
        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<PaymentMethod> PaymentMethods { get; set; }
        public virtual ICollection<Testimonial> Testimonials { get; set; }
        public virtual ICollection<UserrAddress> UserrAddresses { get; set; }
        public virtual ICollection<UserrLogin> UserrLogins { get; set; }
    }
}
