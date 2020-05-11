using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using DotLiquid;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ObjectStorage.DbContext;
using ObjectStorage.MetaModel;
using Property = ObjectStorage.MetaModel.Property;

namespace ObjectStorage
{
    public class Storage
    {
        private ModelDbContext _dbContext;

        public Storage(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ModelDbContext>();
            optionsBuilder.UseSqlite("Data Source=" + connectionString);
            _dbContext = new ModelDbContext(optionsBuilder.Options);
        }

        public void rebuild()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();
        }

        public void add(Class c)
        {
            _dbContext.Classes.Add(c);
            _dbContext.SaveChanges();
        }

        public void load()
        {
            Template.RegisterSafeType(typeof(Class), new[] {"Name", "Properties"});
            Template.RegisterSafeType(typeof(Property), new[] {"Name", "Type"});

            string baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string templatePath = Path.Combine(baseDir, "Template/ClassTemplate.liquid");
            Template template = Template.Parse(File.ReadAllText(templatePath));
            var def = _dbContext.Classes.Include(e => e.Properties).First();
            string classString = template.Render(Hash.FromAnonymousObject(new {data = def}));

            dynamic d = DynamicClassLoader.createDynamicInstance(classString, "GeneratedClass." + def.Name);

            templatePath = Path.Combine(baseDir, "Template/DbContextTemplate.liquid");
            template = Template.Parse(File.ReadAllText(templatePath));
            classString = template.Render(Hash.FromAnonymousObject(new {data = new Class[] {def}}));
            
            d = DynamicClassLoader.createDynamicInstance(classString, "GeneratedClass.GeneratedDbContext");
            Console.Out.WriteLine("d.ty = {0}", d.GetType());
        }
    }
}