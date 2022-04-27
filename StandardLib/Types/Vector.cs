namespace StandardLib.Types;

sealed public class Vector<T> : List<T>
{
    public Vector()
        : base()
    {/* ... */}
    
    public Vector(IEnumerable<T> list)
        : base(list)
    {/* ... */}

    public static Vector<T> operator +(Vector<T> self, T item)
    {
        self.Add(item);
        return self;
    }
    
    public static Vector<T> operator <<(Vector<T> self, int amount)
    {
        return new Vector<T>(self.GetRange(self.Count - amount, amount));
    }
    
    public static Vector<T> operator >>(Vector<T> self, int amount)
    {
        return new Vector<T>(self.GetRange(0, amount));
    }

    public static Vector<T> operator -(Vector<T> self, T item)
    {
        self.Remove(item);
        return self;
    }

    public static Vector<T> operator ~(Vector<T> self)
    {
        self.Reverse();
        return self;
    }

    public static implicit operator Vector<T>(T[] value)
        => new (value);

    public static implicit operator T[](Vector<T> self)
        => self.ToArray();
    
    public static implicit operator Span<T>(Vector<T> self)
        => self.ToArray();
}