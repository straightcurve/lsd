using System.Collections.Generic;

namespace LSD
{
    public interface ISyringe
    {
        void Inject(object instance, Dictionary<string, object> overrides = null);    
    }
}