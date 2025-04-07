namespace Ecommerce.Models
{
    public class OrderProduct
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int OrderId { get; set; }

        public Order Order { get; set; }

        public double Price { get; set; }
        public int Qty { get; set; }
    }
}
