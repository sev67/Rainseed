namespace Planter.Factories.Abstract;

abstract public class AbstractFactory<T> : IAbstractFactory<T>
{
    abstract public T Result();
    
    public static implicit operator T(AbstractFactory<T> self) 
        => self.Result();
}

public interface IAbstractFactory<out T>
{
    T Result();
}