using System;
using System.Collections.Generic;

namespace LSD
{
    public interface ISyringe
    {
        void Inject(object instance, IEnumerable<Override> overrides = null);    
        void InjectRecursively(object instance, IEnumerable<Override> overrides = null);    
    }

    public class Override {
        public Type targetType;
        public Type dependencyType;
        public object dependency;
    }
}