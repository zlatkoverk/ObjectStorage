using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ObjectStorage.MetaModel;

namespace ObjectStorage.DbContext
{
    public class GeneratedDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public GeneratedDbContext() : base(create())
        {
        }

        public static DbContextOptions<GeneratedDbContext> create()
        {
            var optionsBuilder = new DbContextOptionsBuilder<GeneratedDbContext>();
            optionsBuilder.UseSqlite("Data Source=" + "a.db");
            return optionsBuilder.Options;
        }

        public List<object> GetAllTables()
        {
            return new List<object>()
                {};
    }
    }
}