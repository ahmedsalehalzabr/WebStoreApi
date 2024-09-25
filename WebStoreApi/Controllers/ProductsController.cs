using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebStoreApi.Model;
using WebStoreApi.Services;

namespace WebStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext context;
        private readonly IWebHostEnvironment env;

        public ProductsController(AppDbContext context,IWebHostEnvironment env)
        {
            this.context = context;
            this.env = env;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await context.Products.ToListAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromForm]ProductDto productDto)
        {
            if(productDto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "Teh ImageFile is Requird");
                return BadRequest(ModelState);
            }

            // save the image on the server
            string imageFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            imageFileName += Path.GetExtension(productDto.ImageFile.FileName);

            string imagesFolder = env.WebRootPath + "/images/products";

            using (var stream =System.IO.File.Create(imagesFolder + imageFileName))
            {
                productDto.ImageFile.CopyTo(stream);
            }

            // save product in the database
            Product product = new Product()
            {
                Name = productDto.Name,
                Brand = productDto.Brand,
                Category = productDto.Category,
                Price = productDto.Price,
                Description = productDto.Description ?? "",
                ImageFileName = imageFileName,
                CreatedAt = DateTime.Now,
            };
             
            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            return Ok(product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id , [FromForm]ProductDto productDto)
        {
            var product = await context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            string imageFileName = product.ImageFileName;
            if(productDto.ImageFile != null)
            {
                // save the image on the server
                imageFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                imageFileName += Path.GetExtension(productDto.ImageFile.FileName);

                string imagesFolder = env.WebRootPath + "/images/products";
                using (var stream = System.IO.File.Create(imagesFolder + imageFileName))
                {
                    productDto.ImageFile.CopyTo(stream);
                }

                // delete the old image
                System.IO.File.Delete(imagesFolder + product.ImageFileName);

            }

            // update the product in the database
            product.Name = productDto.Name;
            product.Brand = productDto.Brand;
            product.Category = productDto.Category;
            product.Price = productDto.Price;
            product.Description = productDto.Description ?? "";
            product.ImageFileName = imageFileName;

            await context.SaveChangesAsync();

            return Ok(product);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await context.Products.FindAsync(id);
            if (product == null) { return NotFound();}

            // delete the image on the server
            string imagesFolder = env.WebRootPath + "/images/products";
            System.IO.File.Delete(imagesFolder + product.ImageFileName);

            // delete the product from the database
             context.Products.Remove(product);
            await context.SaveChangesAsync();

            return Ok(product);

        }
    }
}
