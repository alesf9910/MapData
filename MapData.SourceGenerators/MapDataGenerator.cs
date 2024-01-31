using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace MapData.SourceGenerators
{
    [Generator]
    public class MapDataGenerator : IIncrementalGenerator
    {
        public HashSet<string> fields_;
        public StringBuilder methods_;
        public StringBuilder class_;
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var provider = context.SyntaxProvider.CreateSyntaxProvider(
                predicate: (node, _) => node is InterfaceDeclarationSyntax,
                transform: (ctx, _) => ctx.Node as InterfaceDeclarationSyntax
            ).Where(m => m != null);

            var compilation = context.CompilationProvider.Combine(provider.Collect());

            context.RegisterSourceOutput(compilation, Execute);
        }
        private void Execute(SourceProductionContext context, (Compilation Left, ImmutableArray<InterfaceDeclarationSyntax> Right) tuple)
        {
            var (compilation, list) = tuple;

            foreach (var st in list)
            {
                var symbol = compilation.GetSemanticModel(st.SyntaxTree)
                    .GetDeclaredSymbol(st) as INamedTypeSymbol;

                if (symbol?.GetAttributes().Select(a => a.AttributeClass.Name == "MapDataAttribute").Count() == 0) continue;
                fields_ = new HashSet<string>();
                methods_ = new StringBuilder();
                class_ = new StringBuilder();
                var className = GetClassCode(symbol);

                var builder = new StringBuilder();
                builder.AppendLine(class_.ToString());

                var methods = symbol?.GetMembers().OfType<IMethodSymbol>();
                if (methods == null) continue;
                foreach (var method in methods)
                {
                    GetMethodCode(method);
                }
                builder.AppendLine(string.Join("\n", fields_));
                builder.AppendLine(methods_.ToString());
                builder.AppendLine("}");
                builder.AppendLine("}");
                context.AddSource($"{className}.g.cs", SourceText.From(builder.ToString(), Encoding.UTF8));
            }
        }
        private string GetClassCode(INamedTypeSymbol namedTypeSymbol)
        {
            var name = namedTypeSymbol.Name.Substring(1);
            var nts = namedTypeSymbol.ContainingNamespace.GetNamespaceMembers().Select(ns => ns.Name);
            class_.AppendLine(GetUsings(namedTypeSymbol));
            class_.AppendLine($"namespace {GetNameSpace(namedTypeSymbol)} {{");
            class_.AppendLine($"public class {name} : {namedTypeSymbol.Name} {{");
            return name;
        }
        private void GetMethodCode(IMethodSymbol method)
        {
            var builder = new StringBuilder();
            var source = method.Parameters[0];
            var destiny = method.ReturnType.Name;
            builder.AppendLine($"public {destiny} {method.Name}({source.Type} {source.Name}){{");
            builder.AppendLine($"{destiny} result = new {destiny}(){{");
            var props = method.ReturnType.GetMembers().OfType<IPropertySymbol>();
            List<string> propsCode = new List<string>();
            foreach (var prop in props)
            {
                var p = GetPropertyCode(prop, source.Type, method.GetAttributes(), source.Name);
                if (p != null) propsCode.Add(p);
            }
            builder.AppendLine(string.Join(",\n", propsCode));
            builder.AppendLine("};");
            builder.AppendLine("return result;");
            builder.AppendLine("}");
            methods_.Append(builder.ToString());
        }
        private string GetPropertyCode(IPropertySymbol property, ITypeSymbol sourceType, ImmutableArray<AttributeData> attrs, string source)
        {
            foreach (var attr in attrs)
            {
                if (attr.AttributeClass.Name == "IgnoreAttribute")
                {

                    var x = attr.ConstructorArguments[0];
                    foreach (var k in x.Values)
                    {
                        if (k.Value.ToString() == property.Name) return null;
                    }
                }
                else if (attr.AttributeClass.Name == "TraductAttribute")
                {
                    var propertyDestiny = attr.ConstructorArguments[0].Value as string;
                    if (propertyDestiny == property.Name)
                    {
                        var propertySource = attr.ConstructorArguments[1].Value as string;
                        return $"{property.Name} = {source}.{propertySource}";
                    }
                }
                else if (attr.AttributeClass.Name == "ConverterAttribute")
                {
                    var propertyDestiny = attr.ConstructorArguments[0].Value as string;
                    if (propertyDestiny == property.Name)
                    {
                        var propertySource = propertyDestiny;
                        var converterName = attr.ConstructorArguments[1].Value as string;
                        if (attr.ConstructorArguments.Count() == 3)
                        {
                            propertySource = attr.ConstructorArguments[1].Value as string;
                            converterName = attr.ConstructorArguments[2].Value as string;
                        }
                        return $"{property.Name} = {AddField(converterName)}.ConvertFrom({source}.{propertySource})";
                    }
                }
            }

            var propMatch = sourceType.GetMembers().OfType<IPropertySymbol>().Where(a => a.Name == property.Name).ToArray();

            if (propMatch.Count() == 0) return null;
            if (!SymbolEqualityComparer.Default.Equals(propMatch[0].Type, property.Type)) return null;
            return $"{property.Name} = {source}.{property.Name}";

        }
        private string GetNameSpace(ITypeSymbol symbol)
        {
            return string.Join(".", symbol.ContainingNamespace.ConstituentNamespaces);
        }
        private string GetUsings(INamedTypeSymbol symbol)
        {
            var builder = new StringBuilder();
            var syntax = symbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax();
            var ancestors = syntax.AncestorsAndSelf();
            foreach (var item in ancestors)
            {
                if (item is CompilationUnitSyntax compilationUnit)
                {
                    foreach (var usg in compilationUnit.Usings)
                    {
                        builder.AppendLine($"using {usg.Name};");
                    }
                }
            }
            return builder.ToString();
        }
        private string AddField(string name){
            string res = name + "_";
            fields_.Add($"private {name} {res} = new {name}();");
            return res;
        }
    }
}
