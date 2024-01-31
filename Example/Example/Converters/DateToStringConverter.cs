using MapData;

namespace Example.Converters;

public class DateToStringConverter : IConverter<DateOnly, string>
{
    public string ConvertFrom(DateOnly source)
    {
        return source.ToString("dd/MM/yyyy");
    }

    public DateOnly ConvertFrom(string source)
    {
        return DateOnly.ParseExact(source, "dd/MM/yyyy", null);
    }
}
