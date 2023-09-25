namespace MapData
{
    public class IgnoreAttribute : Attribute
    {
        string[] attributes;
        public IgnoreAttribute(params string[] attributes)
        {
            this.attributes = attributes;
        }

        public bool hasProperty(string name)
        {
            
            return this.attributes.Contains(name);
        }
    }
}
