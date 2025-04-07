namespace Ecommerce.Configuration
{
    public class RazorPayConfiguration : IRazorPayConfiguration
    {
        public string KeyID { get; set; }
        public string KeySecret { get; set; }
    }

    public interface IRazorPayConfiguration
    {
        public string KeyID { get; set; }
        public string KeySecret { get; set; }
    }
}
