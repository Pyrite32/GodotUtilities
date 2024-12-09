using Microsoft.CodeAnalysis;
using Scriban;

namespace GodotUtilities.SourceGenerators.Scene
{
    [Generator]
    internal class SceneSourceGenerator : SourceGeneratorForDeclaredTypeWithAttribute<GodotUtilities.SceneAttribute>
    {
        private static Template _sceneTreeTemplate;
        private static Template SceneTreeTemplate => _sceneTreeTemplate ??= Template.Parse(Resources.SceneTreeTemplate);

        protected override (string GeneratedCode, DiagnosticDetail Error) GenerateCode(Compilation compilation, SyntaxNode node, INamedTypeSymbol symbol, AttributeData attribute)
        {
            List<LocalDependencyAttributeDataModel> localModels = new();

            foreach (var memberAttribute in GetAllNodeAttributes(symbol))
            {
                switch (memberAttribute.Item1)
                {
                    case IPropertySymbol property:
                        localModels.Add(new LocalDependencyAttributeDataModel(property, memberAttribute.Item2.NodePath));
                        break;
                    case IFieldSymbol field:
                        localModels.Add(new LocalDependencyAttributeDataModel(field, memberAttribute.Item2.NodePath));
                        break;
                }
            }

            var model = new SceneDataModel(symbol) { LocalNodes = localModels };
            var output = SceneTreeTemplate.Render(model, member => member.Name);

            return (output, null);
        }

        private List<(ISymbol, LocalToSceneAttribute)> GetAllNodeAttributes(INamedTypeSymbol symbol, bool excludePrivate = false)
        {
            var result = new List<(ISymbol, LocalToSceneAttribute)>();

            if (symbol.BaseType != null)
            {
                result.AddRange(GetAllNodeAttributes(symbol.BaseType, true));
            }

            var members = symbol.GetMembers()
                .Where(x => !excludePrivate || x.DeclaredAccessibility != Accessibility.Private)
                .Select(member => (member, member.GetAttributes().FirstOrDefault(x => x?.AttributeClass?.Name == nameof(GodotUtilities.LocalToSceneAttribute))))
                .Where(x => x.Item2 != null)
                .Select(x => (x.member, new GodotUtilities.LocalToSceneAttribute((string)x.Item2.ConstructorArguments[0].Value)));

            result.AddRange(members);
            return result;
        }
    }
}
