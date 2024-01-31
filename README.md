# MapData: Una biblioteca para mapear datos

MapData es una biblioteca que utiliza la generación de código fuente en tiempo de compilación para mapear datos de un tipo a otro.

## Cómo usar MapData

Para usar MapData, primero debes definir una interfaz con el atributo `MapDataAttribute`. Los métodos de esta interfaz deben tener un parámetro y devolver un tipo. Aquí tienes un ejemplo:

```csharp
using MapData.Attributes;

[MapData]
public interface IMapData
{
    DestinationType MapMethod(SourceType source);
}
```

### Atributos

MapData proporciona varios atributos que puedes usar para controlar cómo se mapean los datos:

- `IgnoreAttribute`: Este atributo se puede usar en un método para indicar que ciertas propiedades deben ser ignoradas por el generador de código fuente. Puedes especificar múltiples propiedades para ignorar pasándolas como argumentos al constructor del atributo. Aquí tienes un ejemplo:

```csharp
[Ignore("PropertyToIgnore1", "PropertyToIgnore2")]
public DestinationType MapMethod(SourceType source);
```

- `TraductAttribute`: Puedes usar este atributo en un método para indicar que una propiedad debe ser mapeada de una propiedad de origen a una propiedad de destino.

```csharp
[Traduct("DestinationProperty", "SourceProperty")]
public DestinationType MapMethod(SourceType source);
```

- `ConverterAttribute`: Este atributo se puede usar en un método para indicar que un método debe utilizar un convertidor para convertir el valor de una propiedad a otro tipo. El constructor de este atributo puede tomar dos o tres argumentos. Si se pasan dos argumentos, se asume que el nombre de la propiedad de origen es el mismo que el de la propiedad de destino. Aquí tienes un ejemplo:

```csharp
// Usando el mismo nombre para la propiedad de origen y destino
[Converter("Property", "ConverterName")]
public DestinationType MapMethod(SourceType source);

// Especificando nombres diferentes para la propiedad de origen y destino
[Converter("DestinationProperty", "SourceProperty", "ConverterName")]
public DestinationType MapMethod(SourceType source);
```

### Convertidores

Puedes definir tus propios convertidores implementando la interfaz `IConverter<S,D>`. Aquí tienes un ejemplo:

```csharp
public class MyConverter : IConverter<SourceType, DestinationType>
{
    public static DestinationType ConvertFrom(SourceType source)
    {
        // Implementa la lógica de conversión aquí.
    }

    public static SourceType ConvertFrom(DestinationType source)
    {
        // Implementa la lógica de conversión aquí.
    }
}
```

## Generación de código

Cuando compiles tu proyecto, MapData generará automáticamente el código fuente para las clases que implementan tus interfaces. Puedes ver este código en los archivos `.g.cs` generados en la carpeta `obj` de tu proyecto.

## Código de ejemplo

Aquí tienes un ejemplo de cómo podrías usar MapData en tu código:

```csharp
using MapData;
using MapData.Attributes;
using System.Text.Json;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public DateOnly Birthday { get; set; }
    public DateOnly CreatedDay { get; set; }
}

public class UserRegistrationDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public string PasswordConfirmation { get; set; }
    public string Birthday { get; set; }
}

public class UserDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Birthday { get; set; }
}

public class DateToStringConverter : IConverter<DateOnly, string>
{
    public static string ConvertFrom(DateOnly source)
    {
        return source.ToString("dd/MM/yyyy");
    }

    public static DateOnly ConvertFrom(string source)
    {
        return DateOnly.ParseExact(source, "dd/MM/yyyy", null);
    }
}

[MapData]
public interface IUserMapper
{
    [Converter(nameof(User.Birthday), nameof(DateToStringConverter))]
    User UserFromUserRegistration(UserRegistrationDto userRegistrationDto);

    [Converter(nameof(User.Birthday), nameof(DateToStringConverter))]
    UserDetailDto UserDetailFromUser(User user);

    [Ignore(nameof(UserDetailDto.Birthday))]
    [Converter(nameof(User.Birthday), nameof(DateToStringConverter))]
    UserDetailDto UserMinimalDetailFromUser(User user);

}

var registration = new UserRegistrationDto()
{
    Id = 1,
    Name = "Juan Sánchez",
    Birthday = "10/10/1999",
    Password = "U12c10y.",
    PasswordConfirmation = "U12c10y."
};

Console.WriteLine(JsonSerializer.Serialize<UserRegistrationDto>(registration));

IUserMapper userMapper = new UserMapper();

var user = userMapper.UserFromUserRegistration(registration);

Console.WriteLine(JsonSerializer.Serialize<User>(user));

var userDetail = userMapper.UserDetailFromUser(user);

Console.WriteLine(JsonSerializer.Serialize<UserDetailDto>(userDetail));

var userMinimalDetail = userMapper.UserMinimalDetailFromUser(user);

Console.WriteLine(JsonSerializer.Serialize<UserDetailDto>(userMinimalDetail));
```

En este ejemplo, se utiliza `MapData` para mapear datos entre diferentes objetos de usuario. Se definen tres clases: `User`, `UserRegistrationDto` y `UserDetailDto`. Se utiliza un convertidor personalizado, `DateToStringConverter`, para manejar la conversión entre `DateOnly` y `string` para la propiedad `Birthday`. La interfaz `IUserMapper` define tres métodos de mapeo que utilizan el atributo Converter para especificar el uso de `DateToStringConverter` para la propiedad `Birthday`. En el método `UserMinimalDetailFromUser`, se utiliza el atributo `Ignore` para ignorar la propiedad `Birthday` en `UserDetailDto`. Finalmente, se crea un objeto `UserRegistrationDto`, se serializa a JSON, se mapea a un objeto User utilizando `UserFromUserRegistration`, y luego se mapea a `UserDetailDto` utilizando `UserDetailFromUser` y `UserMinimalDetailFromUser`. Cada objeto mapeado se serializa a JSON y se imprime en la consola.