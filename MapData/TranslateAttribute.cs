using System;
using System.Collections.Generic;
using System.Linq;
namespace MapData
{
    public class TranslateAttribute : Attribute
    {
        string sourceProperty;
        string destinyProperty;
        public TranslateAttribute(string sourceProperty, string destinyProperty)
        {
            this.sourceProperty = sourceProperty;
            this.destinyProperty = destinyProperty;
        }

        public bool isDestinyProperty(string name)
        {
            return this.destinyProperty == name;
        }

        public string SourceProperty { get { return this.sourceProperty; } }
    }
}
