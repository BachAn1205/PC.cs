using Microsoft.Build.Framework;

namespace Ecommerce.Models
{
    public class ProductSpecDetail
    {
        public int Id { get; set; }
        [Required]
        public string Key { get; set; }
        [Required]
        public string Value { get; set; }
        public string Unit { get; set; }
        public bool IsHighlighted { get; set; }
        public int SpecGroupId { get; set; }
        public ProductSpecGroup SpecGroup { get; set; }
    }
}
