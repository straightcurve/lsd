using System;
using System.Collections.Generic;

namespace LSD.Creation
{
    public class CloneStrategy : ICreationalStrategy
    {
        private ICloneable original;
        public CloneStrategy(ICloneable original)
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