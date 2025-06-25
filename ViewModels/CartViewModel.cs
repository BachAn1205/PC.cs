namespace Ecommerce.ViewModels
{
    public class CartViewModel
    {
        public List<CartItemViewModel> Items { get; set; }
        public int Count => Items?.Sum(x => x.Quantity) ?? 0;
    }
}
