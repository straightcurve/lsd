using System;

namespace LSD.CreationalStrategies
{
    public class ConstructorCreationalStrategy : ICreationalStrategy
    {
        public object Create(Type type)
        {
            return Activator.CreateInstance(type);
        }
    }
}