using System.Runtime;
using Microsoft.CodeAnalysis;

namespace GodotUtilities.SourceGenerators.Scene
{
    internal class DiTargetAttributeDataModel : MemberDataModel
    {
        public string MemberName { get; }
        public string Path { get; }
        public string Type { get; }
        public string InnerType { get; set; }
        public bool InnerIsNode { get; }
        public bool IsOptionNode { get; }
        public bool IsNode { get; }
        public bool IsScanner { get; }
        internal ISymbol isymbol;


        protected DiTargetAttributeDataModel(ISymbol _typeSymbol, string nodePath) : base(_typeSymbol)
        {
            isymbol = _typeSymbol;
            Path = nodePath;
            // I prefix variables with m and My.
            // for instance: mNode, mThis for fields
            // and:          MyNode, MyThis for properties

            MemberName = _typeSymbol.Name;

            if (_typeSymbol is IFieldSymbol fs)
            {
                Type = fs.Type.ToString();

                var genericPart = "";
                if (Type.Contains('<'))
                {
                    genericPart = Type.Substring(Type.IndexOf('<'));
                }


                SetInnerType(fs.Type, out var nts);
                if (nts != null
                    && nts.TypeArguments.Length > 0
                    && nts.TypeArguments[0].AssignableFrom("Godot.Node"))
                {
                    InnerIsNode = true;
                }

                if (fs.Type.AssignableFrom("Godot.Node"))
                {
                    IsNode = true;
                }
                else if (fs.Type.AssignableFrom("GodotStrict.Types.OptionNode" + genericPart))
                {
                    IsOptionNode = true;
                }
                else if (fs.Type.AssignableFrom("GodotStrict.Types.Scanner" + genericPart))
                {
                    IsScanner = true;
                }
            }
            else if (_typeSymbol is IPropertySymbol ps)
            {
                Type = ps.Type.ToString();
                var genericPart = "";
                if (Type.Contains('<'))
                {
                    genericPart = Type.Substring(Type.IndexOf('<'));
                }

                SetInnerType(ps.Type, out var nts);
                if (nts != null
                    && nts.TypeArguments.Length > 0
                    && nts.TypeArguments[0].AssignableFrom("Godot.Node"))
                {
                    InnerIsNode = true;
                }


                if (ps.Type.AssignableFrom("Godot.Node"))
                {
                    IsNode = true;
                }
                else if (ps.Type.AssignableFrom("GodotStrict.Types.OptionNode" + genericPart))
                {
                    IsOptionNode = true;
                }
                else if (ps.Type.AssignableFrom("GodotStrict.Types.Scanner" + genericPart))
                {
                    IsScanner = true;
                }
            }
        }

        public string InheritanceChain()
        {
            string result = "";
            if (isymbol is IFieldSymbol fs)
            {
                var baseType = fs.Type.BaseType;
                while (baseType != null)
                {
                    result += ", " + baseType.Name;
                    baseType = baseType.BaseType;
                }
            }
            if (isymbol is IPropertySymbol ps)
            {
                var baseType = ps.Type.BaseType;
                while (baseType != null)
                {
                    result += ", " + baseType.Name;
                    baseType = baseType.BaseType;
                }
            }
            return result;
        }


        public DiTargetAttributeDataModel(IPropertySymbol property, string nodePath) : this(property as ISymbol, nodePath)
        {
        }

        public DiTargetAttributeDataModel(IFieldSymbol field, string nodePath) : this(field as ISymbol, nodePath)
        {
        }

        private void SetInnerType(ITypeSymbol typeSymbol, out INamedTypeSymbol _nts)
        {
            _nts = null;
            if (typeSymbol is INamedTypeSymbol nts)
            {
                _nts = nts;
                if (nts.IsGenericType && nts.TypeArguments.Length > 0)
                {
                    _nts = nts;
                    InnerType = nts.TypeArguments[0].ToString();
                    return;
                }
            }
        }
    }
}
