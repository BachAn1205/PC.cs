using Ecommerce.Data;
using Ecommerce.Models;
using Ecommerce.ViewModels;
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

        public IActionResult Details(int id)
        {
            var product = _context.Products
                .Include(p => p.Category)
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            var specs = _context.ProductSpecDetails
                .Where(s => s.SpecGroupId == id)
                .ToList();

            var relatedProducts = _context.Products
                .Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id)
                .Take(4)
                .ToList();

            var viewModel = new ProductDetailViewModel
            {
                Product = product,
                Specifications = specs,
                RelatedProducts = relatedProducts // ✅ Thêm dòng này
            };

            return View(viewModel);
        }
        

    }
}