using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public double Price { get; set; }

        public string Image {  get; set; }

        public string Brand { get; set; }
        public int WarrantyPeriod { get; set; }

        [DisplayName("Category")]
        public int? CategoryId { get; set; }

        public Category Category { get; set; }
    }
}
