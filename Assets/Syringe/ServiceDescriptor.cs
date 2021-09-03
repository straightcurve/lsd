using System;
using UnityEngine;

namespace Syringe {
    internal class ServiceDescriptor {

        public ServiceLifetime Lifetime { get; }
        public object Implementation { get; }
        public Type ServiceType { get; }
        public Type ImplementationType { get; }

        public ServiceDescriptor(object impl, Type serviceType = null) {
            ImplementationType = ServiceType = impl.GetType();
            if (serviceType != null)
                ServiceType = serviceType;

            Implementation = impl;
            Lifetime = ServiceLifetime.Singleton;
        }

        public ServiceDescriptor(Type serviceType, Type implementationType) {
            ServiceType = serviceType;
            ImplementationType = implementationType;
            Implementation = null;
            Lifetime = ServiceLifetime.Transient;
        }
    }
}
