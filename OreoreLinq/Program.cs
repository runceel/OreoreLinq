var items = new ArrayDummyEnumerable<int>(new[] { 1, 2, 3, 4, 5 });
var result = items.Where(x => x % 2 == 0).Select(x => x * x);
while(result.MoveNext())
{
    Console.WriteLine(result.Current);
}


interface IDummyEnumerable<T>
{
    bool MoveNext();
    T Current { get; }
}

class ArrayDummyEnumerable<T> : IDummyEnumerable<T>
{
    private int _currentIndex;
    private readonly T[] _source;

    public ArrayDummyEnumerable(T[] source)
    {
        _source = source;
    }

    public T Current => _source[_currentIndex];

    public bool MoveNext() => ++_currentIndex < _source.Length;
}

class WhereDummyEnumerable<T> : IDummyEnumerable<T>
{
    private readonly IDummyEnumerable<T> _source;
    private Func<T, bool> _predicate;

    public WhereDummyEnumerable(IDummyEnumerable<T> source, Func<T, bool> predicate)
    {
        _source = source;
        _predicate = predicate;
    }

    public T Current => _source.Current;

    public bool MoveNext()
    {
        while(_source.MoveNext())
        {
            if (_predicate(_source.Current))
            {
                return true;
            }
        }

        return false;
    }
}

class SelectDummyEnumerable<T, U> : IDummyEnumerable<U>
{
    private readonly IDummyEnumerable<T> _source;
    private readonly Func<T, U> _converter;

    public SelectDummyEnumerable(IDummyEnumerable<T> source, Func<T, U> converter)
    {
        _source = source;
        _converter = converter;
    }
    public U Current => _converter(_source.Current);

    public bool MoveNext() => _source.MoveNext();
}

static class IDummyEnumerableExtensions
{
    public static IDummyEnumerable<T> Where<T>(this IDummyEnumerable<T> source, Func<T, bool> predicate) =>
        new WhereDummyEnumerable<T>(source ,predicate);

    public static IDummyEnumerable<U> Select<T, U>(this IDummyEnumerable<T> source, Func<T, U> converter) =>
        new SelectDummyEnumerable<T, U>(source, converter);
}