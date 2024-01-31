using System;

namespace MapData.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TraductAttribute : Attribute
    {
        public TraductAttribute(string propertyDestiny, string propertySource)
        {

        }
    }
}
