using System;

namespace MapData
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ConverterAttribute : Attribute
    {
        public ConverterAttribute(string propertyDestiny, string propertySource, string converterName)
        {

        }

        public ConverterAttribute(string property, string converterName)
        {

        }
    }
}
