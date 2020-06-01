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
        private dynamic _storageDbContext;
        private Dictionary<string, List<object>> _tableDictionary = new Dictionary<string, List<object>>();

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

        public void addDefinition(Class c)
        {
            _dbContext.Classes.Add(c);
            _dbContext.SaveChanges();
        }

        public void addElement(string type, Dictionary<string, object> data)
        {
            dynamic c = DynamicClassLoader.createCachedInstance("GeneratedClass." + type);

            foreach (var kvp in data)
            {
                c.GetType().GetProperty(kvp.Key).SetValue(c, kvp.Value, null);
                Console.Out.WriteLine("kvp = {0}", kvp.Value);
            }

            Console.Out.WriteLine("c = {0}", c.Id);
            dynamic dbset = _storageDbContext.GetType().GetProperty(type).GetValue(_storageDbContext, null);
            dbset.GetType().GetMethod("Add").Invoke(dbset, new[] {c});
            _storageDbContext.SaveChanges();
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
                _tableDictionary[def.Name] = new List<object>();
                string classString = template.Render(Hash.FromAnonymousObject(new {data = def}));
                dynamic c = DynamicClassLoader.createDynamicInstance(classString, "GeneratedClass." + def.Name);
                Console.Out.WriteLine("Loaded class {0}", c.GetType());
            }

            templatePath = Path.Combine(baseDir, "Template/DbContextTemplate.liquid");
            template = Template.Parse(File.ReadAllText(templatePath));
            string dbContextClassString = template.Render(Hash.FromAnonymousObject(new {data = classes}));

            _storageDbContext = DynamicClassLoader.createDynamicInstance(dbContextClassString,
                "GeneratedClass.GeneratedDbContext");

            Console.Out.WriteLine("Loaded class {0}", _storageDbContext.GetType());
            _storageDbContext.Database.EnsureCreated();

            try
            {
                foreach (var table in _storageDbContext.GetAllTables())
                {
                    foreach (var a in table.AsQueryable())
                    {
                        string type = a.GetType().ToString();
                        _tableDictionary[type.Split(".")[1]].Add(a);
                        Console.Out.WriteLine("Loaded Object {0}", a.GetType());
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Model has changed - recreating DB");
                _storageDbContext.Database.EnsureDeleted();
                _storageDbContext.Database.EnsureCreated();
            }
        }
    }
}