using Microsoft.EntityFrameworkCore;
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
    }
}
