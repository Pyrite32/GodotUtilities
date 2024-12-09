using Microsoft.CodeAnalysis;

namespace GodotUtilities.SourceGenerators.Scene
{
    internal class SceneDataModel : ClassDataModel
    {
        public List<DiTargetAttributeDataModel> LocalNodes { get; set; }

        public SceneDataModel(INamedTypeSymbol symbol) : base(symbol)
        { }
    }
}
