using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Syringe {
    public class DIContainer : IContainer {

        internal readonly Dictionary<Type, ServiceDescriptor> collection = new Dictionary<Type, ServiceDescriptor>();
        internal DIContainer parent;

        public DIContainer(): this(null) {}
        public DIContainer(DIContainer _parent) {
            this.parent = _parent;

            Register<DIContainer>().FromInstance(this);
        }

        public virtual T Instantiate<T>() {
            return (T)Instantiate(typeof(T));
        }

        public virtual object Instantiate(Type type) {
            var instance = Activator.CreateInstance(type);

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(f => f.GetCustomAttributes(typeof(DependencyAttribute), false).Length > 0)
                .ToList();

            var derivedType = type.BaseType;
            while (derivedType != null) {
                derivedType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(f => f.GetCustomAttributes(typeof(DependencyAttribute), false).Length > 0)
                    .ToList().ForEach(f => fields.Add(f));
                derivedType = derivedType.BaseType;
            }

            foreach (var field in fields)
                field.SetValue(instance, Resolve(field.FieldType));

            return instance;
        }

        public object Resolve(Type type) {
            if (!collection.ContainsKey(type)) {
                if (parent == null)
                    throw new NullReferenceException($"No service of type {type.ToString()}");

                return parent.Resolve(type);
            }

            var descriptor = collection[type];

            try {
                if (descriptor.Implementation != null)
                    return descriptor.Implementation;
                if (descriptor.GetInstance != null)
                    return descriptor.GetInstance();
            } catch (Exception ex) {
                Debug.LogError($"{type} | {descriptor.ImplementationType} | {descriptor.Implementation.GetType()}");
                throw ex;
            }

            return Instantiate(descriptor.ImplementationType);
        }

        [Obsolete("Use Register<TImpl>() or Register<TService, TImpl>() instead", true)]
        internal void RegisterSingleton(Type serviceType, Type implementationType, object implementation)
        {
            var descriptor = new ServiceDescriptor(Instantiate(implementationType), serviceType);
            collection.Add(serviceType, descriptor);
        }

        public TService Resolve<TService>() {
            var type = typeof(TService);

            return (TService)Resolve(type);
        }

        [Obsolete("Use Register<TImpl>() or Register<TService, TImpl>() instead", true)]
        public DIContainer RegisterSingleton<TImpl>() {
            var type = typeof(TImpl);
            var descriptor = new ServiceDescriptor(Instantiate(type));
            collection.Add(type, descriptor);
            return this;
        }

        [Obsolete("Use Register<TImpl>() or Register<TService, TImpl>() instead", true)]
        public DIContainer RegisterSingleton<TImpl>(TImpl implementation) {
            var type = typeof(TImpl);
            var descriptor = new ServiceDescriptor(implementation);
            collection.Add(type, descriptor);
            return this;
        }

        [Obsolete("Use Register<TImpl>() or Register<TService, TImpl>() instead", true)]
        public DIContainer RegisterSingleton<TService, TImpl>() where TImpl: TService {
            var implType = typeof(TImpl);
            var serviceType = typeof(TService);
            var descriptor = new ServiceDescriptor(Instantiate(implType), serviceType);
            collection.Add(serviceType, descriptor);
            return this;
        }

        [Obsolete("Use Register<TImpl>() or Register<TService, TImpl>() instead", true)]
        public DIContainer RegisterSingleton<TService, TImpl>(TImpl implementation) where TImpl: TService {
            var serviceType = typeof(TService);
            var descriptor = new ServiceDescriptor(implementation, serviceType);
            collection.Add(serviceType, descriptor);
            return this;
        }

        [Obsolete("Use Register<TImpl>() or Register<TService, TImpl>() instead", true)]
        public DIContainer RegisterTransient<TImpl>() {
            var type = typeof(TImpl);
            var descriptor = new ServiceDescriptor(type, type);
            collection.Add(type, new ServiceDescriptor(type, type));
            return this;
        }

        [Obsolete("Use Register<TImpl>() or Register<TService, TImpl>() instead", true)]
        public DIContainer RegisterTransient<TService, TImpl>() where TImpl: TService {
            var implType = typeof(TImpl);
            var serviceType = typeof(TService);
            var descriptor = new ServiceDescriptor(serviceType, implType);
            collection.Add(serviceType, descriptor);
            return this;
        }

        public ISourceSelection<TImpl> Register<TImpl>()
        {
            return new Registration<TImpl, TImpl>(this);
        }

        public ISourceSelection<TImpl> Register<TService, TImpl>()
        {
            return new Registration<TService, TImpl>(this);
        }
    }

    public class Registration<TService, TImpl> : ISourceSelection<TImpl>, ILifetimeSelection, IInitializationSelection
    {
        internal DIContainer Container { get; }
        internal ServiceLifetime Lifetime { get; set; }
        internal TImpl Instance { get; private set; }
        internal GameObject Prefab { get; private set; }
        internal ServiceDescriptor Descriptor { get; }
        internal InstantiationType InstantiationType { get; set; }

        public Registration(DIContainer container) {
            Container = container;
            Descriptor = new ServiceDescriptor();
            Container.collection.Add(typeof(TService), Descriptor);
        }

        public ILifetimeSelection FromNew()
        {
            InstantiationType = InstantiationType.FromNew;
            Descriptor.GetInstance = new Syringe.FromNewStrategy<TImpl>(Container).Instantiate;
            return this;
        }

        public void FromInstance(TImpl instance)
        {
            InstantiationType = InstantiationType.FromInstance;
            Instance = instance;
            Descriptor.GetInstance = () => Instance;
        }

        public IInitializationSelection AsSingleton()
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

        public IInitializationSelection AsTransient()
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

        protected Func<object> GetInstance(Func<object> fn, InitializationMethod method) {
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

    public enum InstantiationType {
        FromNew,
        FromInstance,
    }

    public enum InitializationMethod {
        Lazy,
        NonLazy,
    }

    public interface IContainer {
        ISourceSelection<TImpl> Register<TImpl>();
        ISourceSelection<TImpl> Register<TService, TImpl>();

        TService Resolve<TService>();
        object Resolve(Type serviceType);
    }

    public interface ISourceSelection<TImpl>
    {
        ILifetimeSelection FromNew();
        void FromInstance(TImpl instance);
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
