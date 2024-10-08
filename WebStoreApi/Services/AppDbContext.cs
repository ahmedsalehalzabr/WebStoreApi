﻿using Microsoft.EntityFrameworkCore;
using WebStoreApi.Model;

namespace WebStoreApi.Services
{
    public class AppDbContext :DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) 
        {
            
        }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; } 
        public DbSet<PasswordReset> PasswordResets { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Order> Orders { get; set; }

    }
} 
