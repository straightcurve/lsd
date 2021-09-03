namespace Syringe {
    public interface ISourceSelectionStage<TImpl>
    {
        ILifetimeSelectionStage FromNew();
        void FromInstance(TImpl instance);
    }

    public interface ILifetimeSelectionStage
    {
        IInitializationSelectionStage AsSingleton();
        IInitializationSelectionStage AsTransient();
    }

    public interface IInitializationSelectionStage {
        void Lazy();
        void NonLazy();
    }
}
