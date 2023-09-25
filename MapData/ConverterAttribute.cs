namespace MapData
{
    public abstract class ConverterAttribute<S, D> : Attribute
    {
        string[] attributes;

        public ConverterAttribute(params string[] attributes)
        {
            this.attributes = attributes;
        }

        public bool hasProperty(string name)
        {
            return this.attributes.Contains(name);
        }

        public abstract D Convert(S source);
    }
}
