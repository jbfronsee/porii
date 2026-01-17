namespace Lib.Colors;

public interface IColorVector<T>
{
    public T X { get; set; }

    public T Y { get; set; }

    public T Z { get; set; }
}