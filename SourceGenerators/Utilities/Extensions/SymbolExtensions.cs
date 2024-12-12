using Microsoft.CodeAnalysis;

namespace GodotUtilities.SourceGenerators
{
    public static class SymbolExtensions
    {
        public static string NamespaceOrNull(this ISymbol symbol)
            => symbol.ContainingNamespace.IsGlobalNamespace ? null : string.Join(".", symbol.ContainingNamespace.ConstituentNamespaces);

        public static (string NamespaceDeclaration, string NamespaceClosure, string NamespaceIndent) GetNamespaceDeclaration(this ISymbol symbol, string indent = "    ")
        {
            var ns = symbol.NamespaceOrNull();
            return ns is null
                ? (null, null, null)
                : ($"namespace {ns}\n{{\n", "}\n", indent);
        }

        public static INamedTypeSymbol OuterType(this ISymbol symbol)
            => symbol.ContainingType?.OuterType() ?? symbol as INamedTypeSymbol;

        public static string ClassDef(this INamedTypeSymbol symbol)
            => symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

        public static bool InheritsFrom(this ITypeSymbol symbol, string type)
        {
            var baseType = symbol.BaseType;
            if (baseType == null)
            {
                //TODO: for interfaces, return false always.
                return false;
            }
            if (baseType.ToString().Equals(type))
            {
                return true;
            }
            while (baseType != null)
            {
                if (baseType.ToString().Equals(type))
                {
                    return true;
                }

                baseType = baseType.BaseType;
            }

            return false;
        }


        public static bool AssignableFrom(this ITypeSymbol symbol, string type)
        {
            var result = symbol.ToString().Equals(type);
            if (result)
            {
                return true;
            }
            else
            {
                return symbol.InheritsFrom(type);
            }
        }

    }
}
