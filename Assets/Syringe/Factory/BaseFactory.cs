namespace Syringe
{
    public class BaseFactory<T> : IFactory<T>
    {
        [Dependency]
        protected DIContainer container;

        public virtual T Create() => container.Instantiate<T>();
    }
}
