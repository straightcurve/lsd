using System;
using System.Collections.Generic;

namespace LSD.Creation
{
    public class ConstructorStrategy : ICreationalStrategy
    {
        private ISyringe syringe;

        public ConstructorStrategy(ISyringe syringe)
        {
            this.syringe = syringe;
        }

        public object Create(Type type, IEnumerable<Override> overrides = null)
        {
            var instance = Activator.CreateInstance(type);
            syringe.Inject(instance, overrides);
            return instance;
        }

        public TImpl Create<TImpl>(IEnumerable<Override> overrides = null)
        {
            var instance = Activator.CreateInstance<TImpl>();
            syringe.Inject(instance, overrides);
            return instance;
        }

        public object CreateRecursively(Type type, IEnumerable<Override> overrides = null)
        {
            var instance = Activator.CreateInstance(type);
            syringe.InjectRecursively(instance, overrides);
            return instance;
        }

        public TImpl CreateRecursively<TImpl>(IEnumerable<Override> overrides = null)
        {
            var instance = Activator.CreateInstance<TImpl>();
            syringe.InjectRecursively(instance, overrides);
            return instance;
        }
    }
}