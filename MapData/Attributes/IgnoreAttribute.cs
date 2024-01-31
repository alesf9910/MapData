using System;

namespace MapData.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class IgnoreAttribute : Attribute
    {
        public IgnoreAttribute(params string[] propertyName)
        {
        }
    }
}
