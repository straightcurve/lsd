using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Syringe {
    public class UnityDIContainer : DIContainer, IRegister {

        public UnityDIContainer(): this(null) {}
        public UnityDIContainer(DIContainer _parent) {
            this.parent = _parent;
        }

        public override T Instantiate<T>() {
            return (T)Instantiate(typeof(T));
        }

        public override object Instantiate(Type type) {
            if (type.IsSubclassOf(typeof(Component))) {
                var instance = new GameObject().AddComponent(type);

                var fields = type.GetFields(
                    BindingFlags.Public
                    | BindingFlags.NonPublic
                    | BindingFlags.Instance)
                    .Where(f => f.GetCustomAttributes(typeof(DependencyAttribute), false).Length > 0)
                    .ToArray();
            
                foreach (var field in fields)
                    field.SetValue(instance, Resolve(field.FieldType));

                return instance;
            }

            return base.Instantiate(type);
        }

        public class Registration<TService, TImpl> : ISourceSelection<TImpl>, ILifetimeSelection, IInitializationSelection
        {
            internal DIContainer Container { get; }
            internal ServiceLifetime Lifetime { get; set; }
            internal TImpl Instance { get; private set; }
            internal GameObject Prefab { get; private set; }
            internal ServiceDescriptor Descriptor { get; }

            public Registration(DIContainer container) {
                Container = container;
                Descriptor = new ServiceDescriptor();
                Container.collection.Add(typeof(TService), Descriptor);
            }

            public ILifetimeSelection FromNew()
            {
                throw new NotImplementedException();
            }

            public ILifetimeSelection FromPrefab(GameObject prefab)
            {
                Prefab = prefab;
                return this;
            }

            public ILifetimeSelection FromInstance(TImpl instance)
            {
                Instance = instance;
                Descriptor.GetInstance = () => {
                    return instance;
                };
                return this;
            }

            public IInitializationSelection AsSingleton()
            {
                Lifetime = ServiceLifetime.Singleton;
                return this;
            }

            public IInitializationSelection AsTransient()
            {
                Lifetime = ServiceLifetime.Transient;
                return this;
            }

            public void Lazy()
            {
                throw new NotImplementedException();
            }

            public void NonLazy()
            {
                // this is the default strategy
            }
        }

        public ISourceSelection<TImpl> Register<TImpl>()
        {
            var registration = new Registration<TImpl, TImpl>(this);

            return registration;
        }

        public ISourceSelection<TImpl> Register<TService, TImpl>()
        {
            var registration = new Registration<TService, TImpl>(this);

            return registration;
        }
    }

    public interface IRegister {
        ISourceSelection<TImpl> Register<TImpl>();
        ISourceSelection<TImpl> Register<TService, TImpl>();
    }

    public interface ISourceSelection<TImpl>
    {
        ILifetimeSelection FromNew();
        ILifetimeSelection FromPrefab(GameObject gameObject);
        ILifetimeSelection FromInstance(TImpl gameObject);
    }

    public interface ILifetimeSelection
    {
        IInitializationSelection AsSingleton();
        IInitializationSelection AsTransient();
    }

    public interface IInitializationSelection {
        void Lazy();
        void NonLazy();
    }
}
