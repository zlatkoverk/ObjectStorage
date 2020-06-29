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
        private Dictionary<string, List<dynamic>> _tableDictionary = new Dictionary<string, List<dynamic>>();

        public Storage(ModelDbContext dbContext)
        {
            _dbContext = dbContext;
            build();
            load();
        }

        public void build()
        {
            // _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();
        }

        public void addDefinition(Class c)
        {
            var o = _dbContext.Classes.Find(c.Name);
            if (o == null)
            {
                _dbContext.Classes.Add(c);
            }
            else
            {
                o.OverviewTemplate = c.OverviewTemplate;
                o.DetailsTemplate = c.DetailsTemplate;
            }

            _dbContext.SaveChanges();
        }

        public void addProperty(string className, Property property)
        {
            var c = _dbContext.Classes.Find(className);
            c.Properties.Add(property);
            _dbContext.SaveChanges();
        }

        public Class deleteProperty(Guid id)
        {
            var prop = _dbContext.Properties.Find(id);
            var c = _dbContext.Classes.AsEnumerable().Single(c => c.Properties.Contains(prop));
            _dbContext.Properties.Remove(prop);
            c.Properties.Remove(prop);
            _dbContext.SaveChanges();
            return c;
        }

        public List<Class> getClasses()
        {
            return _dbContext.Classes.Include(c => c.Properties).ThenInclude(p => p.Constraint).ToList();
        }

        public List<object> getEntities(Class c)
        {
            return _tableDictionary[c.Name];
        }

        public void add(Class c)
        {
            _dbContext.Classes.Add(c);
            _dbContext.SaveChanges();
        }

        public bool addElement(string type, Dictionary<string, string> data)
        {
            dynamic c = DynamicClassLoader.createCachedInstance("GeneratedClass." + type);
            var cl = getClasses().Find(e => e.Name == type);

            foreach (var kvp in data)
            {
                var prop = cl.Properties.First(e => e.Name == kvp.Key);
                Type t = c.GetType().GetProperty(kvp.Key).PropertyType;
                bool isPrimitiveType = t.IsPrimitive || t.IsValueType || (t == typeof(string));

                if (!isPrimitiveType)
                {
                    var id = Guid.Parse(kvp.Value);
                    dynamic d = _tableDictionary[prop.Type].Find(p => p.Id.Equals(id));
                    c.GetType().GetProperty(kvp.Key)
                        .SetValue(c, Convert.ChangeType(d, c.GetType().GetProperty(kvp.Key).PropertyType),
                            null);
                    Console.Out.WriteLine("kvp = {0}", id);
                }
                else
                {
                    dynamic val;
                    try
                    {
                        val = Convert.ChangeType(kvp.Value, c.GetType().GetProperty(kvp.Key).PropertyType);
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }

                    c.GetType().GetProperty(kvp.Key)
                        .SetValue(c, val,
                            null);
                    var constraint = cl.Properties.Single(p => p.Name == kvp.Key).Constraint;
                    if (constraint != null && (val < constraint.MinValue || val > constraint.MaxValue))
                    {
                        return false;
                    }

                    Console.Out.WriteLine("kvp = {0}", kvp.Value);
                }
            }

            Console.Out.WriteLine("c = {0}", c.Id);
            dynamic dbset = _storageDbContext.GetType().GetProperty(type).GetValue(_storageDbContext, null);
            dbset.GetType().GetMethod("Add").Invoke(dbset, new[] {c});
            _storageDbContext.SaveChanges();
            return true;
        }

        public bool editElement(string type, Dictionary<string, string> data, string entityId)
        {
            dynamic c = _tableDictionary[type].Find(e => e.Id.ToString().Equals(entityId));
            var cl = getClasses().Find(e => e.Name == type);

            foreach (var kvp in data)
            {
                var prop = cl.Properties.First(e => e.Name == kvp.Key);
                Type t = c.GetType().GetProperty(kvp.Key).PropertyType;
                bool isPrimitiveType = t.IsPrimitive || t.IsValueType || (t == typeof(string));

                if (!isPrimitiveType)
                {
                    var id = Guid.Parse(kvp.Value);
                    dynamic d = _tableDictionary[prop.Type].Find(p => p.Id.Equals(id));
                    c.GetType().GetProperty(kvp.Key)
                        .SetValue(c, Convert.ChangeType(d, c.GetType().GetProperty(kvp.Key).PropertyType),
                            null);
                    Console.Out.WriteLine("kvp = {0}", id);
                }
                else
                {
                    dynamic val;
                    try
                    {
                        val = Convert.ChangeType(kvp.Value, c.GetType().GetProperty(kvp.Key).PropertyType);
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }

                    c.GetType().GetProperty(kvp.Key)
                        .SetValue(c, val,
                            null);
                    var constraint = cl.Properties.Single(p => p.Name == kvp.Key).Constraint;
                    if (constraint != null && (val < constraint.MinValue || val > constraint.MaxValue))
                    {
                        return false;
                    }
                    Console.Out.WriteLine("kvp = {0}", kvp.Value);
                }
            }

            Console.Out.WriteLine("c = {0}", c.Id);
            _storageDbContext.SaveChanges();
            return true;
        }

        public void removeElement(string type, string id)
        {
            Guid guid;
            if (!Guid.TryParse(id, out guid))
            {
                return;
            }

            var element = _tableDictionary[type].Find(v => v.Id == guid);

            dynamic dbset = _storageDbContext.GetType().GetProperty(type).GetValue(_storageDbContext, null);
            dbset.GetType().GetMethod("Remove").Invoke(dbset, new[] {element});
            _storageDbContext.SaveChanges();
        }

        public void load()
        {
            Template.RegisterSafeType(typeof(Class), new[] {"Name", "DisplayName", "Properties", "OverviewTemplate"});
            Template.RegisterSafeType(typeof(Property), new[] {"Name", "DisplayName", "Type"});

            string baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string templatePath = Path.Combine(baseDir, "Template/ClassTemplate.liquid");
            Template template = Template.Parse(File.ReadAllText(templatePath));

            var classes = _dbContext.Classes.Include(e => e.Properties).ToList();
            loadAssemblies(classes, template);

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

        private void loadAssemblies(List<Class> classes, Template template)
        {
            if (classes.Count == 0)
            {
                return;
            }

            var skipped = new List<Class>();
            foreach (var def in classes)
            {
                _tableDictionary[def.Name] = new List<dynamic>();
                string classString = template.Render(Hash.FromAnonymousObject(new
                    {data = def, presentationProperty = def.PresentationProperty}));
                dynamic c = DynamicClassLoader.createDynamicInstance(classString, "GeneratedClass." + def.Name);
                if (c == null)
                {
                    skipped.Add(def);
                    continue;
                }

                Console.Out.WriteLine("Loaded class {0}", c.GetType());
            }

            loadAssemblies(skipped, template);
        }
    }
}