using System.Diagnostics;
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
        public bool LookAtSibling { get; }
        internal ISymbol isymbol;


        protected DiTargetAttributeDataModel(ISymbol _typeSymbol, string nodePath, bool isSibling) : base(_typeSymbol)
        {
            //File.AppendAllText("C:\\Users\\patri\\Programming\\output-template.csx", "\nenter");
            isymbol = _typeSymbol;

            LookAtSibling = isSibling;

            Path = nodePath;

            // I prefix variables with m and My.
            // for instance: mNode, mThis for fields
            // and:          MyNode, MyThis for properties

            MemberName = _typeSymbol?.Name ?? "";

            if (_typeSymbol is IFieldSymbol fs)
            {
                //File.AppendAllText("C:\\Users\\patri\\Programming\\output-template.csx", "\nenter try block");
                try
                {

                    Type = fs.Type?.ToString();
                    //File.AppendAllText("C:\\Users\\patri\\Programming\\output-template.csx", "\nassigned type");
                    if (Type == null || Type == "")
                    {
                        //File.AppendAllText("C:\\Users\\patri\\Programming\\output-template.csx", "\nFIELD_TYPE IS NULL");
                        throw new InvalidOperationException("Type is null");
                    }

                    //File.AppendAllText("C:\\Users\\patri\\Programming\\output-template.csx", "\nPast null type check");
                    var textGenericIdentifier = "";
                    if (Type.Contains('<'))
                    {
                        textGenericIdentifier = Type.Substring(Type.IndexOf('<'));
                    }
                    //File.AppendAllText("C:\\Users\\patri\\Programming\\output-template.csx", "\nadjusted string");

                    if (SetInnerType(fs.Type, out var nts))
                    {
                        //File.AppendAllText("C:\\Users\\patri\\Programming\\output-template.csx", "\nRan SetInnerType");
                        if (nts != null
                            && nts.TypeArguments != null
                            && nts.TypeArguments.Length > 0
                            && nts.TypeArguments[0].AssignableFrom("Godot.Node"))
                        {
                            InnerIsNode = true;
                        }
                    }

                    //File.AppendAllText("C:\\Users\\patri\\Programming\\output-template.csx", "\nInner Is Node Check Done");
                    if (fs.Type == null)
                    {
                        //File.AppendAllText("C:\\Users\\patri\\Programming\\output-template.csx", "\n fs.Type is null.");
                        throw new InvalidProgramException("fs.Type is null.");
                    }
                    else
                    {
                        //File.AppendAllText("C:\\Users\\patri\\Programming\\output-template.csx", "fs.Type == " + fs.Type);
                    }

                    if (fs.Type.AssignableFrom("Godot.Node"))
                    {
                        IsNode = true;
                    }
                    else if (fs.Type.AssignableFrom("GodotStrict.Types.Option" + textGenericIdentifier))
                    {
                        IsOptionNode = true;
                    }
                    else if (fs.Type.AssignableFrom("GodotStrict.Types.Scanner" + textGenericIdentifier))
                    {
                        IsScanner = true;
                    }
                    //File.AppendAllText("C:\\Users\\patri\\Programming\\output-template.csx", "\nDone!\n\n");
                }
                catch (NullReferenceException n)
                {
                    var stackTrace = new StackTrace(n, true);
                    var frame = stackTrace?.GetFrame(0);
                    var line = frame?.GetFileLineNumber();

                    //File.AppendAllText("C:\\Users\\patri\\Programming\\output-template.csx", "\nFIELD" + n.StackTrace + '\n' + (line ?? -999));

                    throw new InvalidOperationException("An error occurred when checking field");
                }
            }
            else if (_typeSymbol is IPropertySymbol ps)
            {
                try
                {
                    Type = ps.Type.ToString();
                    var genericPart = "";
                    if (Type.Contains('<'))
                    {
                        genericPart = Type.Substring(Type.IndexOf('<'));
                    }

                    if (SetInnerType(ps.Type, out var nts))
                    {
                        if (nts != null
                            && nts.TypeArguments.Length > 0
                            && nts.TypeArguments[0].AssignableFrom("Godot.Node"))
                        {
                            InnerIsNode = true;
                        }
                    }


                    if (ps.Type.AssignableFrom("Godot.Node"))
                    {
                        IsNode = true;
                    }
                    else if (ps.Type.AssignableFrom("GodotStrict.Types.Option" + genericPart))
                    {
                        IsOptionNode = true;
                    }
                    else if (ps.Type.AssignableFrom("GodotStrict.Types.Scanner" + genericPart))
                    {
                        IsScanner = true;
                    }
                }
                catch (NullReferenceException n)
                {

                    //File.AppendAllText("C:\\Users\\patri\\Programming\\output-template.csx", "\nPROPERTY" + n.StackTrace);

                    throw new InvalidOperationException("An error occurred when checking property: " + n.Message);
                }
            }
        }

        public string InheritanceChain()
        {
            try
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
            catch (NullReferenceException n)
            {
                //File.AppendAllText("C:\\Users\\patri\\Programming\\output-template.csx", n.StackTrace + "INH CHAIN");
                throw new InvalidOperationException("An error occurred when resolving the inheritance chain");
                throw n;
            }
        }


        public DiTargetAttributeDataModel(IPropertySymbol property, string nodePath, bool isSibling) : this(property as ISymbol, nodePath, isSibling)
        {
        }

        public DiTargetAttributeDataModel(IFieldSymbol field, string nodePath, bool isSibling) : this(field as ISymbol, nodePath, isSibling)
        {
        }

        private bool SetInnerType(ITypeSymbol typeSymbol, out INamedTypeSymbol _nts)
        {
            try
            {
                _nts = null;
                if (typeSymbol is INamedTypeSymbol nts)
                {
                    _nts = nts;
                    if (nts.IsGenericType && nts.TypeArguments.Length > 0)
                    {
                        _nts = nts;
                        InnerType = nts.TypeArguments[0].ToString();
                        return true;
                    }
                }
                return false;
            }
            catch (NullReferenceException n)
            {
                //File.AppendAllText("C:\\Users\\patri\\Programming\\output-template.csx", n.Message);
                throw n;
            }
        }
    }
}
