namespace GodotUtilities
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class AutowiredAttribute : Attribute
    {
        public string NodePath { get; }

        public AutowiredAttribute(string nodePath = null)
        {
            if (nodePath != null && !nodePath.Equals(string.Empty))
            {
                if (nodePath.Equals("@"))
                {
                    lookAtSiblings = true;
                    return;
                }
                else if (nodePath.StartsWith("@"))
                {
                    nodePath = nodePath.Substring(1);
                    lookAtSiblings = true;
                }
                NodePath = '"' + nodePath + '"';
            }
        }

        internal bool lookAtSiblings = false;
    }
}
