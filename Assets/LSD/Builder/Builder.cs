using System;
using System.Collections.Generic;
using LSD.CreationalStrategies;

namespace LSD.Builder
{
    public class Builder<TImpl> : IBuilder<TImpl>
    {
        [Dependency]
        protected ISyringe syringe;
        protected readonly List<Override> overrides = new List<Override>();
        protected ICreationalStrategy strategy;

        public virtual IBuilder<TImpl> FromNew()
        {
            strategy = new ConstructorCreationalStrategy();
            return this;
        }

        public IBuilder<TImpl> Override<TDependency, TIn>(TDependency dependency)
        {
            overrides.Add(new Override { targetType = typeof(TIn), dependencyType = typeof(TDependency), dependency = dependency });
            return this;
        }

        /// <summary>
        /// Inject overrides
        /// </summary>
        /// <returns></returns>
        public virtual TImpl Build()
        {
            var instance = (TImpl)strategy.Create(typeof(TImpl));

            // we need to inject recursively
            syringe.InjectRecursively(instance, overrides);
            overrides.Clear();

            return instance;
        }
    }
}