using Ecommerce.Models;

namespace Ecommerce.ViewModels
{
    public class ProductDetailViewModel
    {
        public Product Product { get; set; }
        public List<ProductSpecDetail> Specifications { get; set; }
    }
}
