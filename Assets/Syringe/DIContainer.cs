using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Syringe {
    public class DIContainer {

        internal readonly Dictionary<Type, ServiceDescriptor> collection = new Dictionary<Type, ServiceDescriptor>();
        internal DIContainer parent;

        public DIContainer(): this(null) {}
        public DIContainer(DIContainer _parent) {
            this.parent = _parent;

            RegisterSingleton<DIContainer>(this);
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
            } catch (Exception ex) {
                Debug.LogError($"{type} | {descriptor.ImplementationType} | {descriptor.Implementation.GetType()}");
                throw ex;
            }
            
            return Instantiate(descriptor.ImplementationType);
        }

        public TService Resolve<TService>() {
            var type = typeof(TService);

            return (TService)Resolve(type);
        }

        public DIContainer RegisterSingleton<TImpl>() {
            var type = typeof(TImpl);
            var descriptor = new ServiceDescriptor(Instantiate(type));
            collection.Add(type, descriptor);
            return this;
        }

        public DIContainer RegisterSingleton<TImpl>(TImpl implementation) {
            var type = typeof(TImpl);
            var descriptor = new ServiceDescriptor(implementation);
            collection.Add(type, descriptor);
            return this;
        }

        public DIContainer RegisterSingleton<TService, TImpl>() where TImpl: TService {
            var implType = typeof(TImpl);
            var serviceType = typeof(TService);
            var descriptor = new ServiceDescriptor(Instantiate(implType), serviceType);
            collection.Add(serviceType, descriptor);
            return this;
        }

        public DIContainer RegisterSingleton<TService, TImpl>(TImpl implementation) where TImpl: TService {
            var serviceType = typeof(TService);
            var descriptor = new ServiceDescriptor(implementation, serviceType);
            collection.Add(serviceType, descriptor);
            return this;
        }

        public DIContainer RegisterTransient<TImpl>() {
            var type = typeof(TImpl);
            var descriptor = new ServiceDescriptor(type, type);
            collection.Add(type, new ServiceDescriptor(type, type));
            return this;
        }

        public DIContainer RegisterTransient<TService, TImpl>() where TImpl: TService {
            var implType = typeof(TImpl);
            var serviceType = typeof(TService);
            var descriptor = new ServiceDescriptor(serviceType, implType);
            collection.Add(serviceType, descriptor);
            return this;
        }
    }
}
