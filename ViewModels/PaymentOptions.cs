namespace Ecommerce.ViewModels
{
    public class PaymentOptions
    {
        public string razorpayKey { get; set; }
        public double amount { get; set; }
        public string currency { get; set; }
        public string name { get; set; }
        public string orderId { get; set; }
        public string email { get; set; }
        public string contactNumber { get; set; }
        public int addressId { get; set; }
    }
}
