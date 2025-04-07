using Ecommerce.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị tất cả sản phẩm nhóm theo Category
        public async Task<IActionResult> Index()
        {
            var categoriesWithProducts = await _context.Categories
                .Include(c => c.Products)
                .Where(c => c.Products.Any())
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(categoriesWithProducts);
        }

        // Hiển thị sản phẩm theo một Category cụ thể
        public async Task<IActionResult> ByCategory(int categoryId)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == categoryId);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category) // Load thông tin Category nếu cần
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
    }
}