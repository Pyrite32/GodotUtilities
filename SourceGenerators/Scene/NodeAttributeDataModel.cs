using Microsoft.CodeAnalysis;
using GodotUtilities.CaseExtensions;

namespace GodotUtilities.SourceGenerators.Scene
{
    internal class LocalDependencyAttributeDataModel : MemberDataModel
    {
        public string Path { get; }
        public string PascalName { get; }
        public string SnakeName { get; }
        public string LowerName { get; }
        public string MemberName { get; }
        public string CamelName { get; }
        public string Type { get; }
        public string InnerType { get; }
        public bool InnerIsNode { get; }

        protected LocalDependencyAttributeDataModel(ISymbol typeSymbol, string nodePath) : base(typeSymbol)
        {
            Path = nodePath;
            // I prefix variables with m and My.
            // for instance: mNode, mThis for fields
            // and:          MyNode, MyThis for properties
            MemberName = typeSymbol.Name.FromMyCase().FromMCase();
            PascalName = MemberName.ToPascalCase();
            SnakeName = MemberName.ToSnakeCase();
            LowerName = MemberName.ToLowerInvariant();
            CamelName = MemberName.ToCamelCase();

            if (typeSymbol is IFieldSymbol fs)
            {
                InnerType = GetInnerType(fs.Type, out var nts);
                if (nts != null 
                    && nts.TypeArguments.Length > 0
                    && nts.TypeArguments[0].InheritsFrom("Godot.Node"))
                {
                    InnerIsNode = true;
                }
            }
            else if (typeSymbol is IPropertySymbol ps)
            {
                InnerType = GetInnerType(ps.Type, out var nts);
                if (nts != null 
                    && nts.TypeArguments.Length > 0
                    && nts.TypeArguments[0].InheritsFrom("Godot.Node"))
                {
                    InnerIsNode = true;
                }
            }
        }

        public LocalDependencyAttributeDataModel(IPropertySymbol property, string nodePath) : this(property as ISymbol, nodePath)
        {
            Type = property.Type.ToString();
            
        }

        public LocalDependencyAttributeDataModel(IFieldSymbol field, string nodePath) : this(field as ISymbol, nodePath)
        {
            Type = field.Type.ToString();
        }

        private string GetInnerType(ITypeSymbol typeSymbol, out INamedTypeSymbol _nts)
        {
            _nts = null;
            if (typeSymbol is INamedTypeSymbol nts)
            {
                _nts = nts;
                if (nts.IsGenericType && nts.TypeArguments.Length > 0)
                {
                    _nts = nts;
                    return nts.TypeArguments[0].ToString();
                }
            }
            return "";
        }
    }
}
