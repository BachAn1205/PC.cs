using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Models
{
    public class Address
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public string AddresssLine {  get; set; }   

        public string City { get; set; }

        public string District { get; set; }

        public string Ward { get; set; }
    }
}
