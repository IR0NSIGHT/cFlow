public interface IMapIterator<T>
{
    IEnumerable<T> Points();
    IEnumerable<IEnumerable<T>> PointsByRow();
    IEnumerable<T> Row(int y);
}