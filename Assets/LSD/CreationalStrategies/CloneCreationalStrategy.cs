using System;
using System.Collections.Generic;

namespace LSD.CreationalStrategies
{
    public class CloneCreationalStrategy : ICreationalStrategy
    {
        private ICloneable original;
        public CloneCreationalStrategy(ICloneable original)
        {
            this.original = original;
        }

        public object Create(Type type, IEnumerable<Override> overrides = null)
        {
            return original.Clone();
        }

        public TImpl Create<TImpl>(IEnumerable<Override> overrides = null)
        {
            return (TImpl)original.Clone();
        }

        public object CreateRecursively(Type type, IEnumerable<Override> overrides = null)
        {
            return original.Clone();
        }

        public TImpl CreateRecursively<TImpl>(IEnumerable<Override> overrides = null)
        {
            return (TImpl)original.Clone();
        }
    }
}