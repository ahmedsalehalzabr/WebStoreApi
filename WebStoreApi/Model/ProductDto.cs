﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WebStoreApi.Model
{
    public class ProductDto
    {
        [Required,MaxLength(100)]
        public string Name { get; set; } = "";
        [Required, MaxLength(100)]
        public string Brand { get; set; } = "";
        public int CategoryId { get; set; }
        [Required]
        public decimal Price { get; set; }
        [MaxLength(4000)]
        public string? Description { get; set; }
      
        public IFormFile? ImageFile { get; set; }
    }
}
