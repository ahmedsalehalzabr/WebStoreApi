﻿using Microsoft.AspNetCore.Http;
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

        [HttpGet("category")]
        public async Task<IActionResult> GetCategories()
        {
            var listCategory = await context.Categories.ToListAsync();
            return Ok(listCategory);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts(string? search, string? category,
            int? minPrice, int? maxPrice,
            string? sort, string? order,
            int? page)
        {
            IQueryable<Product> query = context.Products.Include(c => c.Category);
            if (search != null)
            {
                query = query.Where(P => P.Name.Contains(search) || P.Description.Contains(search));
            }

            if (category != null)
            {
                query = query.Where(p => p.Category.Name == category);
            }

            if (minPrice != null)
            {
                query = query.Where(p => p.Price == minPrice);  
            }

            if (maxPrice != null)
            {
                query = query.Where(p => p.Price == maxPrice);
            }

            // sort functionality
            if (sort == null) sort = "id";
            if (order == null || order != "asc") order = "desc";
            if (sort.ToLower() == "name")
            {
                if (order == "asc")
                {
                    query = query.OrderBy(p => p.Name);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Name);
                }
            }
            else if (sort.ToLower() == "brand")
            {
                if (order == "asc")
                {
                    query = query.OrderBy(p => p.Brand);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Brand);
                }
            }
            else if (sort.ToLower() == "category")
            {
                if (order == "asc")
                {
                    query = query.OrderBy(p => p.Category.Name);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Category.Name);
                }
            }
            else if (sort.ToLower() == "price")
            {
                if (order == "asc")
                {
                    query = query.OrderBy(p => p.Price);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Price);
                }
            }
            else if (sort.ToLower() == "data")
            {
                if (order == "asc")
                {
                    query = query.OrderBy(p => p.CreatedAt);
                }
                else
                {
                    query = query.OrderByDescending(p => p.CreatedAt);
                }
            }
            else 
            {
                if (order == "asc")
                {
                    query = query.OrderBy(p => p.Id);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Id);
                }
            }

            // pagination functionality
            if (page == null || page < 1) page = 1;
            int pageSize = 5;
            int totalPages = 0;

            decimal count = query.Count();
            totalPages = (int)Math.Ceiling(count / pageSize);

            query = query.Skip((int) (page - 1) * pageSize).Take(pageSize);


            var products = await query.ToListAsync();

            var response = new
            { 
               Products = products,
               TotalPages = totalPages,
               PageSize = pageSize,
               Page = page,
            };
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await context.Products.Include(c => c.Category).FirstOrDefaultAsync(c => c.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromForm]ProductDto productDto)
        {
            var category = await context.Categories.FindAsync(productDto.CategoryId);
            if (category == null)
            {
                ModelState.AddModelError("Category", "Please select a valid category");
                return BadRequest(ModelState);
            }

            if (productDto.ImageFile == null)
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
                Category = category,
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
            var category = await context.Categories.FindAsync(productDto.CategoryId);
            if (category == null)
            {
                ModelState.AddModelError("Category", "Please select a valid category");
                return BadRequest(ModelState);
            }

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
            product.Category = category;
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
