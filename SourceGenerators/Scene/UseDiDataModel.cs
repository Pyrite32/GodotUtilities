using Microsoft.CodeAnalysis;

namespace GodotUtilities.SourceGenerators.Scene
{
    internal class UseDiDataModel : ClassDataModel
    {
        public List<DiTargetAttributeDataModel> LocalNodes { get; set; }

        public UseDiDataModel(INamedTypeSymbol symbol) : base(symbol)
        { }
    }
}
