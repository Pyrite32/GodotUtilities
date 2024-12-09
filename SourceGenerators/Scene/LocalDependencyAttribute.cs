namespace GodotUtilities
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class LocalToSceneAttribute : Attribute
    {
        public string NodePath { get; }

        public LocalToSceneAttribute(string nodePath = null)
        {
            NodePath = nodePath;
        }
    }
}
