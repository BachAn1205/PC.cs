using Ecommerce.Models;

namespace Ecommerce.ViewModels
{
    public class ProductDetailViewModel
    {
        public Product Product { get; set; }
        public List<Product> RelatedProducts { get; set; }  // <--- Thêm dòng này

        public List<ProductSpecDetail> Specifications { get; set; }
    }
}
