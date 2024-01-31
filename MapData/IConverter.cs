namespace MapData
{
    public interface IConverter<D, S>
    {
        D ConvertFrom(S source);
        S ConvertFrom(D source);
    }
}
