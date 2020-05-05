using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel;

namespace SourceGeneratorSamples
{
    [Generator]
    public class HelloWorldGenerator : ISourceGenerator
    {
        public void Execute(SourceGeneratorContext context)
        {
            // begin creating the source we'll inject into the users compilation
            var sourceBuilder = new StringBuilder(@"
using NUnit.Framework;
namespace HelloWorldGenerated
{
    public static class HelloWorld
    {
        public static void SayHello() 
        {
            TestContext.WriteLine(""Hello from generated code!"");
            TestContext.WriteLine(""Listen to me!"");
            TestContext.WriteLine(""The following syntax trees existed in the compilation that created this program:"");
");

            var attributes = context.Compilation.Assembly.GetAttributes();
            foreach (var attribute in attributes)
            {
                var attributeClass = attribute.AttributeClass;
                if (attributeClass is null) continue;
                sourceBuilder.AppendLine($@"TestContext.WriteLine(@"" - {attributeClass.Name}"");");
                if ("RegisterTargetTypeAttribute" == attributeClass.Name)
                {
                    var c0 = attribute.ConstructorArguments[0];
                    var c1 = attribute.ConstructorArguments[1];
                    var t = c0.Value as INamedTypeSymbol;
                    var i = c1.Value;
                    if (i is null) continue;
                    sourceBuilder.AppendLine($@"TestContext.WriteLine(@""   - YYYY {t?.Name} - {(int)i}"");");
                }
            }

            // finish creating the source to inject
            sourceBuilder.Append(@"
        }
    }
}");

            // inject the created source into the users compilation
            context.AddSource("helloWorldGenerator", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
        }

        public void Initialize(InitializationContext context)
        {
            // No initialization required for this one
        }
    }
}