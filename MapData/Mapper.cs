using System.Reflection;

namespace MapData
{
    public abstract class Mapper
    {
        Dictionary<(Type, Type), MethodInfo> types { get; set; }

        public Mapper()
        {
            this.types = new Dictionary<(Type, Type), MethodInfo>();
        }

        public D Map<S,D>(S source)
        {
            D destiny = Activator.CreateInstance<D>();
            Type sType = typeof(S);
            Type dType = typeof(D);
            MethodInfo method;
            if (types.ContainsKey((sType, dType)))
            {
                method = types[(sType, dType)];
            }
            else
            {
                foreach (MethodInfo methodInfo in this.GetType().GetMethods())
                {
                    if (methodInfo.ReturnType == dType && methodInfo.GetParameters()[0].ParameterType == sType)
                    {
                        types[(sType, dType)] = methodInfo;
                        break;
                    }
                }
                method = types[(sType, dType)];
            }
            bool shouldBreak;
            foreach(PropertyInfo property in dType.GetProperties())
            {
                shouldBreak = false;
                string propertyName = property.Name;
                foreach (TranslateAttribute translate in method.GetCustomAttributes<TranslateAttribute>())
                {
                    if (translate.isDestinyProperty(propertyName)) propertyName = translate.SourceProperty;
                }
                object value = sType.GetProperty(propertyName).GetValue(source);
                foreach (object attribute in method.GetCustomAttributes(false))
                {
                    if(attribute is IgnoreAttribute ignore)
                    {
                        if (ignore.hasProperty(propertyName))
                        {
                            shouldBreak = true;
                            break;
                        }
                    }else if(attribute.GetType().BaseType.IsGenericType && attribute.GetType().BaseType.GetGenericTypeDefinition() == typeof(ConverterAttribute<,>))
                    {
                        Type convert = attribute.GetType();
                        if((bool)convert.GetMethod("hasProperty").Invoke(attribute, new object[] { propertyName }))
                            value = convert.GetMethod("Convert").Invoke(attribute, new object[] { value });
                    }
                }
                if (shouldBreak) continue;
                dType.GetProperty(property.Name).SetValue(destiny, value);
            }
            return destiny;
        }
    }
}
