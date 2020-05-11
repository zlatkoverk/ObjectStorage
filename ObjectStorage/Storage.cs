using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ObjectStorage.DbContext;
using ObjectStorage.MetaModel;

namespace ObjectStorage
{
    public class ObjectStorage
    {
        private List<Class> _classes = new List<Class>();
        private ModelDbContext _dbContext;

        public ObjectStorage(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ModelDbContext>();
            optionsBuilder.UseSqlite("Data Source=" + connectionString);
            _dbContext = new ModelDbContext(optionsBuilder.Options);
        }

        public void update()
        {
            _dbContext.Database.Migrate();
        }
    }
}