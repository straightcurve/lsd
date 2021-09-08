using System;
using System.Collections.Generic;
using UnityEngine;

namespace LSD
{
    internal class ServiceDescriptor
    {

        public ServiceLifetime Lifetime { get; }
        public object Implementation { get; internal set; }
        public Func<object> GetInstance { get; internal set; }
        public Func<IEnumerable<Override>, object> GetOverridenInstance { get; internal set; }

        public ServiceDescriptor(object impl)
        {
            Implementation = impl;
            Lifetime = ServiceLifetime.Singleton;
        }

        public ServiceDescriptor()
        {
            Implementation = null;
            Lifetime = ServiceLifetime.Transient;
        }
    }
}
