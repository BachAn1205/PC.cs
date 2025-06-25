using Ecommerce.Configuration;
using Ecommerce.Data;
using Ecommerce.Data.Migrations;
using Ecommerce.Models;
using Ecommerce.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Razorpay.Api;

namespace Ecommerce.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRazorPayConfiguration _razorPay;

        public CheckoutController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IRazorPayConfiguration razorPay)
        {
            _context = context;
            _userManager = userManager;
            _razorPay = razorPay;
        }


        public async Task<IActionResult> Index()
        {
            var currentuser = await _userManager.GetUserAsync(HttpContext.User);

            var addresses = await _context.Addresses
                .Include(x => x.User)
                .Where(x => x.UserId == currentuser.Id) 
                .ToListAsync();

            ViewBag.Addresses = addresses;
            ViewBag.FullName = currentuser.FullName ?? "Không có thông tin";
            ViewBag.Email = currentuser.Email ?? "Chưa cập nhật email";
            ViewBag.PhoneNumber = currentuser.PhoneNumber ?? "Chưa cập nhật SĐT";

            var cart = await _context.Carts
                .Include(x => x.Product)
                .Where(x => x.UserId == currentuser.Id)
                .ToListAsync();

            double totalCost = 0;

            foreach (var cartItem in cart)
            {
                totalCost += cartItem.Product.Price * cartItem.Qty;
            }

            ViewBag.TotalCost = totalCost;

            
            return View();
        }

        // Action để hiển thị form chỉnh sửa (nếu cần)
        [HttpGet]
        public async Task<IActionResult> EditAddress(int id)
        {
            var currentuser = await _userManager.GetUserAsync(HttpContext.User);
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == currentuser.Id);

            if (address == null)
            {
                return NotFound();
            }

            return View(address); // Hoặc trả về PartialView nếu dùng modal
        }

        // Action để xử lý cập nhật địa chỉ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAddress(int id, Address address)
        {
            if (id != address.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var currentuser = await _userManager.GetUserAsync(HttpContext.User);
                    address.UserId = currentuser.Id;
                    _context.Update(address);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AddressExists(address.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(address);
        }

        // API endpoint cho AJAX edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAddressAjax([FromBody] Address address)
        {
            if (ModelState.IsValid)
            {
                var currentuser = await _userManager.GetUserAsync(HttpContext.User);
                var existingAddress = await _context.Addresses
                    .FirstOrDefaultAsync(a => a.Id == address.Id && a.UserId == currentuser.Id);

                if (existingAddress == null)
                {
                    return NotFound();
                }

                existingAddress.AddresssLine = address.AddresssLine;
                existingAddress.Ward = address.Ward;
                existingAddress.District = address.District;
                existingAddress.City = address.City;

                _context.Update(existingAddress);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }

            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });
        }

        // Action để xóa địa chỉ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var currentuser = await _userManager.GetUserAsync(HttpContext.User);
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == currentuser.Id);

            if (address == null)
            {
                return NotFound();
            }

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // API endpoint cho AJAX delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAddressAjax(int id)
        {
            var currentuser = await _userManager.GetUserAsync(HttpContext.User);
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == currentuser.Id);

            if (address == null)
            {
                return Json(new { success = false, error = "Address not found" });
            }

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        private bool AddressExists(int id)
        {
            return _context.Addresses.Any(e => e.Id == id);
        }

        // Các action khác giữ nguyên...
       

        public async Task<IActionResult> PaymentOptions(int addressId)
        {
            if (_razorPay == null || string.IsNullOrEmpty(_razorPay.KeyID) || string.IsNullOrEmpty(_razorPay.KeySecret))
            {
                throw new Exception("RazorPay configuration is missing or invalid.");
            }

            var address = await _context.Addresses.Where(x => x.Id == addressId).FirstOrDefaultAsync();
            if (address == null)
            {
                return BadRequest();
            }

            var currentuser = await _userManager.GetUserAsync(HttpContext.User);

            double orderCost = 0;

            var carts = await _context.Carts
                .Include(x => x.Product)
                .Where(x => x.UserId == currentuser.Id).ToListAsync();

            foreach (var cart in carts)
            {
                orderCost += (cart.Product.Price * cart.Qty);
            }

            var transactionId= Guid.NewGuid().ToString();
            RazorpayClient client = new RazorpayClient(_razorPay.KeyID, _razorPay.KeySecret);

            Dictionary<string, object> options = new Dictionary<string, object>();

            options.Add("amount", orderCost * 100);
            options.Add("receipt", transactionId);
            options.Add("currency", "INR");
            options.Add("payment_capture", "0");

            Razorpay.Api.Order orderResponse= client.Order.Create(options);

            string orderId = orderResponse["id"].ToString();

            var paymentOptions = new PaymentOptions
            {
                addressId = addressId,
                orderId = orderId,
                razorpayKey= _razorPay.KeyID,
                amount= orderCost * 100,
                currency = "INR",
                name = currentuser.FullName,
                email = currentuser.Email,
                contactNumber = currentuser.PhoneNumber,
            };

            ViewBag.items = orderCost;
            ViewBag.tax = 0;
            ViewBag.delivery = 0;
            ViewBag.subtotal = orderCost;
            ViewBag.ordertotal = orderCost;

            return View(paymentOptions);
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Complete(string rzp_paymentid, string rzp_orderid, int addressId)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

            if (user != null)
            {
                var carts = await _context.Carts
                    .Include(x => x.Product)
                    .Where(x => x.UserId == user.Id).ToListAsync();

                string paymentId = rzp_paymentid;
                string orderId = rzp_orderid;

                Razorpay.Api.RazorpayClient client = new Razorpay.Api.RazorpayClient(_razorPay.KeyID, _razorPay.KeySecret);
                Razorpay.Api.Payment payment = client.Payment.Fetch(paymentId);

                Dictionary<string, object> options = new Dictionary<string, object>();
                options.Add("amount", payment.Attributes["amount"]);
                Razorpay.Api.Payment paymentCaptured = payment.Capture(options);
                string amt = paymentCaptured.Attributes["amount"];
                var amount = double.Parse(amt);

                var order = new Models.Order
                {
                    AddressId = addressId,
                    CreatedAt = DateTime.Now,
                    UserId = user.Id,
                    Amount = amount / 100,
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Tạo danh sách OrderProducts
                var orderProducts = new List<OrderProduct>();
                foreach (var cart in carts)
                {
                    orderProducts.Add(new OrderProduct
                    {
                        ProductId = cart.ProductId,
                        OrderId = order.Id,
                        Price = cart.Product.Price,
                        Qty = cart.Qty,
                    });
                }

                // Thêm tất cả OrderProducts cùng lúc
                await _context.OrderProducts.AddRangeAsync(orderProducts);
                await _context.SaveChangesAsync();

                // Xóa giỏ hàng
                _context.Carts.RemoveRange(carts);
                await _context.SaveChangesAsync();
            }

              return RedirectToAction("ThankYou");
        }

        public IActionResult ThankYou()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(Address address)
        {
            if (ModelState.IsValid)
            {
                var currentuser = await _userManager.GetUserAsync(HttpContext.User);

                address.UserId = currentuser.Id;
                _context.Addresses.Add(address);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(address);
        }
    }
}
