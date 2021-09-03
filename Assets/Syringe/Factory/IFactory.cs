namespace Syringe
{
    public interface IFactory<T> {
        T Create();
    }
}
