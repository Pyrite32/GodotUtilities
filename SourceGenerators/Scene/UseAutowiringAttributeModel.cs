using Microsoft.CodeAnalysis;

namespace GodotUtilities.SourceGenerators.Scene
{
    internal class UseAutowiringAttributeDataModel : ClassDataModel
    {
        public List<DiTargetAttributeDataModel> LocalNodes { get; set; }

        public UseAutowiringAttributeDataModel(INamedTypeSymbol symbol) : base(symbol)
        { }
    }
}
