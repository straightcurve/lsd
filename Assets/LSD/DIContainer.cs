using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace LSD {
    public class DIContainer {

        internal readonly Dictionary<Type, ServiceDescriptor> collection = new Dictionary<Type, ServiceDescriptor>();
        internal DIContainer parent;

        public DIContainer(): this(null) {}
        public DIContainer(DIContainer _parent) {
            this.parent = _parent;

            Register<DIContainer>().FromInstance(this);
        }

        internal virtual void Inject(object instance) {
            var type = instance.GetType();
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

        public ISourceSelectionStage<TImpl> Register<TImpl>()
        {
            return new Registration<TImpl, TImpl>(this);
        }

        public ISourceSelectionStage<TImpl> Register<TService, TImpl>()
        {
            return new Registration<TService, TImpl>(this);
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

        public TService Resolve<TService>() {
            var type = typeof(TService);

            return (TService)Resolve(type);
        }
    }
}