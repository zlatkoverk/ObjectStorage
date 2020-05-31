using System;
using System.CodeDom.Compiler;
using System.Collections;
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

            var classes = _dbContext.Classes.Include(e => e.Properties).ToList();
            foreach (var def in classes)
            {
                string classString = template.Render(Hash.FromAnonymousObject(new {data = def}));
                dynamic c = DynamicClassLoader.createDynamicInstance(classString, "GeneratedClass." + def.Name);
                Console.Out.WriteLine("Loaded class {0}", c.GetType());
            }

            templatePath = Path.Combine(baseDir, "Template/DbContextTemplate.liquid");
            template = Template.Parse(File.ReadAllText(templatePath));
            string dbContextClassString = template.Render(Hash.FromAnonymousObject(new {data = classes}));

            dynamic d = DynamicClassLoader.createDynamicInstance(dbContextClassString,
                "GeneratedClass.GeneratedDbContext");

            Console.Out.WriteLine("Loaded class {0}", d.GetType());
            d.Database.EnsureCreated();

            try
            {
                foreach (var table in d.GetAllTables())
                {
                    foreach (var a in table.AsQueryable())
                    {
                        Console.Out.WriteLine("Loaded Object {0}", a.GetType());
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Model has changed - recreating DB");
                d.Database.EnsureDeleted();
                d.Database.EnsureCreated();
            }
        }
    }
}