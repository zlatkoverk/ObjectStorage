using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ObjectStorage;
using ObjectStorage.MetaModel;
using Property = ObjectStorage.MetaModel.Property;

namespace ObjectStorageConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            // var s = new Storage(
                // "random.db");
            // s.load();
            // s.addElement("MyClass", new Dictionary<string, object>()
            // {
                // {"p1", 2},
                // {"Id", Guid.NewGuid()}
            // });
            return;
            const string code =
                    "using System;using System.IO;using System.ComponentModel.DataAnnotations;using System.ComponentModel.DataAnnotations.Schema;namespace GeneratedClasses{[Table(\"tableName\")]public class Helper{public int func(){return 4;}}}"
                ;
            string baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string templatePath = Path.Combine(baseDir, "Template/ClassTemplate.liquid");
            dynamic o = DynamicClassLoader.createDynamicInstance(code, "GeneratedClasses.Helper");
            Console.Out.WriteLine(o.func());
        }
    }
}