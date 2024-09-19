using Microsoft.EntityFrameworkCore;

namespace WebStoreApi.Services
{
    public class AppDbContext :DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) 
        {
            
        }
    }
}
