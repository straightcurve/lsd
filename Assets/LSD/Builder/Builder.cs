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

        public Builder()
        {
            New();
        }

        public IBuilder<TImpl> Override<TDependency, TIn>(TDependency dependency)
        {
            overrides.Add(new Override { targetType = typeof(TIn), dependencyType = typeof(TDependency), dependency = dependency });
            return this;
        }

        public virtual TImpl Build()
        {
            var instance = strategy.CreateRecursively<TImpl>(overrides);
            overrides.Clear();
            return instance;
        }

        public virtual IBuilder<TImpl> New()
        {
            strategy = new ConstructorCreationalStrategy(syringe);
            return this;
        }

        public virtual IBuilder<TImpl> Clone<TOriginal>(TOriginal instance) where TOriginal : ICloneable, TImpl
        {
            if (instance == null) throw new ArgumentNullException("instance");

            strategy = new CloneCreationalStrategy(instance);
            return this;
        }
    }
}