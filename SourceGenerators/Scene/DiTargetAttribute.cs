namespace GodotUtilities
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class DiTargetAttribute : Attribute
    {
        public string NodePath { get; }

        public DiTargetAttribute(string nodePath = null)
        {
            if (nodePath != null)
            {
                NodePath = '"' + nodePath + '"';
            }
        }
    }
}
