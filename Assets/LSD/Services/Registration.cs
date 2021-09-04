
using System;

namespace LSD {
    public class Registration<TService, TImpl> : ISourceSelectionStage<TImpl>, ILifetimeSelectionStage, IInitializationSelectionStage
    {
        internal ISyringe Syringe { get; }
        internal ServiceLifetime Lifetime { get; set; }
        internal TImpl Instance { get; private set; }
        internal ServiceDescriptor Descriptor { get; }

        public Registration(ISyringe syringe) {
            Syringe = syringe;
            Descriptor = new ServiceDescriptor();
        }

        public ILifetimeSelectionStage FromNew()
        {
            Descriptor.GetInstance = () => {
                var instance = Activator.CreateInstance<TImpl>();

                Syringe.Inject(instance);

                return instance;
            };
            return this;
        }

        public void FromInstance(TImpl instance)
        {
            Instance = instance;
            Descriptor.GetInstance = () => Instance;
        }

        public IInitializationSelectionStage AsSingleton()
        {
            Lifetime = ServiceLifetime.Singleton;
            var fn = Descriptor.GetInstance;
            Descriptor.GetInstance = () => {
                if (Descriptor.Implementation == null)
                    Descriptor.Implementation = fn();

                return Descriptor.Implementation;
            };
            return this;
        }

        public IInitializationSelectionStage AsTransient()
        {
            Lifetime = ServiceLifetime.Transient;
            return this;
        }

        public void Lazy()
        {
            Descriptor.GetInstance = GetInstance(Descriptor.GetInstance, InitializationMethod.Lazy);
        }

        public void NonLazy()
        {
            Descriptor.GetInstance = GetInstance(Descriptor.GetInstance, InitializationMethod.NonLazy);
        }

        internal Func<object> GetInstance(Func<object> fn, InitializationMethod method) {
            switch (method) {
                case InitializationMethod.Lazy:
                default: {
                    return fn;
                }

                case InitializationMethod.NonLazy: {
                    var instance = fn();
                    return () => instance;
                }
            }
        }
    }
}
