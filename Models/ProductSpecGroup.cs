using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
    public class ProductSpecGroup
    {
        public int Id { get; set; }
        [Required]
        public string GroupName { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public ICollection<ProductSpecDetail> Specifications { get; set; }
    }
}
