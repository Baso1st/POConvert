using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace POConvertAPI.Services
{
    public static class CSharpToJson
    {
        /// <summary>
        /// Compiles C# code and creates instances of object types
        /// </summary>
        /// <param name="csharp">C# code text</param>
        /// <returns>Collection of object instances</returns>
        public static IEnumerable<object> CompileClasses(string csharp)
        {
            if (string.IsNullOrEmpty(csharp))
            {
                throw new ArgumentNullException(nameof(csharp));
            }

            SyntaxTree tree = CSharpSyntaxTree.ParseText(csharp);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
        
            // add Using statements to syntax tree
            var system = SyntaxFactory.IdentifierName("System");
            var systemCollections = SyntaxFactory.QualifiedName(system, SyntaxFactory.IdentifierName("Collections"));
            var systemCollectionsGeneric = SyntaxFactory.QualifiedName(systemCollections, SyntaxFactory.IdentifierName("Generic"));
            var systemLinq = SyntaxFactory.QualifiedName(system, SyntaxFactory.IdentifierName("Linq"));
            var systemText = SyntaxFactory.QualifiedName(system, SyntaxFactory.IdentifierName("Text"));            
            var systemXml = SyntaxFactory.QualifiedName(system, SyntaxFactory.IdentifierName("Xml"));

            var declaredUsings = root.Usings.Select(x => x.Name.ToString()).ToList();
            if (!declaredUsings.Contains("System"))
            {
                root = root.AddUsings(SyntaxFactory.UsingDirective(system).NormalizeWhitespace());
            }
            if (!declaredUsings.Contains("System.Collections"))
            {
                root = root.AddUsings(SyntaxFactory.UsingDirective(systemCollections).NormalizeWhitespace());
            }
            if (!declaredUsings.Contains("System.Collections.Generic"))
            {
                root = root.AddUsings(SyntaxFactory.UsingDirective(systemCollectionsGeneric).NormalizeWhitespace());
            }
            if (!declaredUsings.Contains("System.Linq"))
            {
                root = root.AddUsings(SyntaxFactory.UsingDirective(systemText).NormalizeWhitespace());
            }
            if (!declaredUsings.Contains("System.Text"))
            {
                root = root.AddUsings(SyntaxFactory.UsingDirective(systemLinq).NormalizeWhitespace());
            }            
            if (!declaredUsings.Contains("System.Xml"))
            {
               root = root.AddUsings(SyntaxFactory.UsingDirective(systemXml).NormalizeWhitespace());
            }

            tree = CSharpSyntaxTree.Create(root);
            root = tree.GetCompilationUnitRoot();

            // generate compiled object with references to commonly used .NET Framework assemblies
            var compilation = CSharpCompilation.Create("CSharp2Json",
                syntaxTrees: new[] {tree},
                references: new[]
                {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),      // mscorelib.dll
                    MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),  // System.Core.dll
                    MetadataReference.CreateFromFile(typeof(Uri).Assembly.Location),         // System.dll
                    MetadataReference.CreateFromFile(typeof(DataSet).Assembly.Location),     // System.Data.dll
                    MetadataReference.CreateFromFile(typeof(EntityKey).Assembly.Location),   // System.Data.Entity.dll
                    MetadataReference.CreateFromFile(typeof(XmlDocument).Assembly.Location), // System.Xml.dll
                },
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            );


      // load compiled bits into assembly
      Assembly assembly;
            using (var memoryStream = new MemoryStream())
            {
                var result = compilation.Emit(memoryStream);
                if (!result.Success)
                {
                    //throw new RoslynException(result.Diagnostics);
                    throw new Exception(String.Join(@"\n\n", result.Diagnostics.ToList()));
                }

                assembly = AppDomain.CurrentDomain.Load(memoryStream.ToArray());
            }

            // instantiate object instances from assembly types
            foreach (var definedType in assembly.DefinedTypes)
            {
                Type objType = assembly.GetType(definedType.FullName);
                if (objType.BaseType?.FullName != "System.Enum")
                {
                    object instance = null;
                    try
                    {
                        instance = assembly.CreateInstance(definedType.FullName);
                    }
                    catch (MissingMethodException)
                    {
                        // no default constructor - eat the exception
                    }

                    if (instance != null)
                    {
                        yield return instance;
                    }
                }
            }
        }
    }
}
