namespace Ecommerce.Models
{
    public class UserInfoViewModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public List<Address> Addresses { get; set; }
    }

}
