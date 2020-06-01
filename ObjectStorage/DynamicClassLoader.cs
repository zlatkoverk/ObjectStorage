using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace ObjectStorage
{
    public class DynamicClassLoader
    {
        private static Dictionary<string, Assembly> assms = new Dictionary<string, Assembly>();
        private static Dictionary<string, string> classes = new Dictionary<string, string>();

        public static object createCachedInstance(string className)
        {
            return createDynamicInstance(classes[className], className);
        }

        public static object createDynamicInstance(string code, string className)
        {
            if (assms.ContainsKey(className))
            {
                return assms[className].CreateInstance(className);
            }
            var tree = SyntaxFactory.ParseSyntaxTree(code);
            string fileName = "mylib" + className + ".dll";

            // Detect the file location for the library that defines the object type
            var systemRefLocation = typeof(object).GetTypeInfo().Assembly.Location;

            // Create a reference to the library
            var refs = new List<MetadataReference>()
            {
                MetadataReference.CreateFromFile(systemRefLocation),
                MetadataReference.CreateFromFile(typeof(TableAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(DbSetInitializer).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(SqlitePropertyExtensions).Assembly.Location),
                // MetadataReference.CreateFromFile(Assembly.Load("System.Xml.Linq").Location),
                MetadataReference.CreateFromFile(Assembly.Load("netstandard, Version=2.0.0.0").Location),
                MetadataReference.CreateFromFile(Assembly.Load("System.Runtime, Version=4.2.2.0").Location),
                MetadataReference.CreateFromFile(Assembly.Load("System.Data.Common, Version=4.2.2.0").Location),
                //TODO: HERE ADD STUFF
            };
            refs.AddRange(assms.Select(a => MetadataReference.CreateFromFile(a.Value.Location)));

            // A single, immutable invocation to the compiler
            // to produce a library
            var compilation = CSharpCompilation.Create(fileName)
                .WithOptions(
                    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(refs)
                .AddSyntaxTrees(tree);
            string path = Path.Combine(Directory.GetCurrentDirectory(), fileName);

            EmitResult compilationResult = compilation.Emit(path);
            if (compilationResult.Success)
            {
                // Load the assembly
                Assembly asm =
                    AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
                assms[className] = asm;
                // Invoke the RoslynCore.Helper.CalculateCircleArea method passing an argument
                double radius = 10;
                dynamic r = asm.CreateInstance(className);
                classes[className] = code;
                return r;
                // object result =
                // asm.GetType("RoslynCore.Helper").GetMethod("func")
                // .Invoke(null, new object[] {radius});
                // Console.WriteLine($"Circle area with radius = {radius} is {result}");
            }

            foreach (Diagnostic codeIssue in compilationResult.Diagnostics)
            {
                string issue = $"ID: {codeIssue.Id}, Message: {codeIssue.GetMessage()}, " +
                               $"Location: {codeIssue.Location.GetLineSpan()}," +
                               $" Severity: {codeIssue.Severity}";
                Console.WriteLine(issue);
            }

            return null;
        }
    }
}