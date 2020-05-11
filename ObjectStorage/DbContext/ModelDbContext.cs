using Microsoft.EntityFrameworkCore;
using ObjectStorage.MetaModel;

namespace ObjectStorage.DbContext
{
    public class ModelDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbSet<Class> Classes { get; set; }
        public DbSet<Property> Properties { get; set; }

        public ModelDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}