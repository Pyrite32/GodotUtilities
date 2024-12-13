using Microsoft.CodeAnalysis;
using Scriban;

namespace GodotUtilities.SourceGenerators.Scene
{
    [Generator]
    internal class SceneSourceGenerator : SourceGeneratorForDeclaredTypeWithAttribute<GodotUtilities.UseDiAttribute>
    {
        private static Template _sceneTreeTemplate;
        private static Template SceneTreeTemplate => _sceneTreeTemplate ??= Template.Parse(Resources.SceneTreeTemplate);

        protected override (string GeneratedCode, DiagnosticDetail Error) GenerateCode(Compilation compilation, SyntaxNode node, INamedTypeSymbol symbol, AttributeData attribute)
        {
            try
            {

                List<DiTargetAttributeDataModel> localModels = new();

                foreach (var memberAttribute in GetAllNodeAttributes(symbol))
                {
                    switch (memberAttribute.Item1)
                    {
                        case IPropertySymbol property:
                            localModels.Add(new DiTargetAttributeDataModel(property, memberAttribute.Item2.NodePath));
                            break;
                        case IFieldSymbol field:
                            localModels.Add(new DiTargetAttributeDataModel(field, memberAttribute.Item2.NodePath));
                            break;
                    }
                }

                var model = new UseDiDataModel(symbol) { LocalNodes = localModels };
                var output = SceneTreeTemplate.Render(model, member => member.Name);
                var fileOutput = output;
                foreach (var m in localModels)
                {
                    fileOutput += "\n" + $"nodepath: {m.Path} | type: {m.Type} | inner type: {m.InnerType} | inner type is node: {m.InnerIsNode} | is node: {m.IsNode} | is option: {m.IsOptionNode} | is scanner: {m.IsScanner}";
                    fileOutput += "\n\t i-chain: " + m.InheritanceChain();
                }
                return (output, null);
            }
            catch (NullReferenceException n)
            {
                File.WriteAllText("C:\\Users\\patri\\Programming\\output-template.csx", n.StackTrace);
                throw n;
            }
        }

        private List<(ISymbol, DiTargetAttribute)> GetAllNodeAttributes(INamedTypeSymbol symbol, bool excludePrivate = false)
        {
            try
            {
                var result = new List<(ISymbol, DiTargetAttribute)>();

                if (symbol.BaseType != null)
                {
                    result.AddRange(GetAllNodeAttributes(symbol.BaseType, true));
                }

                var members = symbol.GetMembers()
                    .Where(x => !excludePrivate || x.DeclaredAccessibility != Accessibility.Private)
                    .Select(member => (member, member.GetAttributes().FirstOrDefault(x => x?.AttributeClass?.Name == nameof(GodotUtilities.DiTargetAttribute))))
                    .Where(x => x.Item2 != null)
                    .Select(x => (x.member, new GodotUtilities.DiTargetAttribute((string)x.Item2.ConstructorArguments[0].Value)));

                result.AddRange(members);
                return result;
            }
            catch (NullReferenceException n)
            {
                File.WriteAllText("C:\\Users\\patri\\Programming\\output-template.csx", n.StackTrace);
                throw n;
            }
        }
    }
}
