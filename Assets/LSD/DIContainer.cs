using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace LSD
{
    public class DIContainer
    {

        internal readonly Dictionary<Type, ServiceDescriptor> collection = new Dictionary<Type, ServiceDescriptor>();
        internal DIContainer parent;
        protected ISyringe syringe;

        public DIContainer() : this(null) { }
        public DIContainer(DIContainer _parent)
        {
            this.parent = _parent;
            syringe = new Syringe(this);

            Register<DIContainer>().FromInstance(this);
            Register<ISyringe>().FromInstance(syringe);
        }

        public ISourceSelectionStage<TImpl> Register<TImpl>()
        {
            return Register<TImpl, TImpl>();
        }

        public ISourceSelectionStage<TImpl> Register<TService, TImpl>()
        {
            var reg = new Registration<TService, TImpl>(syringe);
            collection.Add(typeof(TService), reg.Descriptor);
            return reg;
        }

        internal object Resolve(Type type, IEnumerable<Override> overrides)
        {
            if (overrides == null)
                throw new ArgumentNullException("overrides");

            if (!collection.ContainsKey(type))
            {
                if (parent == null)
                    throw new NullReferenceException($"No service of type {type.ToString()}");

                return parent.Resolve(type, overrides);
            }

            var descriptor = collection[type];

            if (descriptor.Implementation != null)
                return descriptor.Implementation;
            if (descriptor.GetOverridenInstance != null)
                return descriptor.GetOverridenInstance(overrides);

            throw new NullReferenceException($"No service of type {type.ToString()}");
        }

        public object Resolve(Type type)
        {
            if (!collection.ContainsKey(type))
            {
                if (parent == null)
                    throw new NullReferenceException($"No service of type {type.ToString()}");

                return parent.Resolve(type);
            }

            var descriptor = collection[type];

            if (descriptor.Implementation != null)
                return descriptor.Implementation;
            if (descriptor.GetInstance != null)
                return descriptor.GetInstance();

            throw new NullReferenceException($"No service of type {type.ToString()}");
        }

        public TService Resolve<TService>()
        {
            var type = typeof(TService);

            return (TService)Resolve(type);
        }
    }
}
