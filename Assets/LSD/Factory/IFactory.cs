namespace LSD
{
    public interface IFactory<T> {
        T Create();
    }
}
