using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LSD.CreationalStrategies
{
    public class CloneComponentStrategy : ICreationalStrategy
    {
        private ICloneable original;

        public CloneComponentStrategy(ICloneable original)
        {
            this.original = original;
        }

        public object Create(Type type, IEnumerable<Override> overrides = null)
        {
            var instance = original.Clone();

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(f => f.GetCustomAttributes(typeof(DependencyAttribute), false).Length > 0)
                .ToList();

            var derivedType = type.BaseType;
            while (derivedType != null)
            {
                derivedType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(f => f.GetCustomAttributes(typeof(DependencyAttribute), false).Length > 0)
                    .ToList().ForEach(f => fields.Add(f));
                derivedType = derivedType.BaseType;
            }

            foreach (var field in fields)
                field.SetValue(instance, field.GetValue(original));

            return instance;
        }

        public TImpl Create<TImpl>(IEnumerable<Override> overrides = null)
        {
            return (TImpl)Create(typeof(TImpl), overrides);
        }

        public object CreateRecursively(Type type, IEnumerable<Override> overrides = null)
        {
            return Create(type, overrides);
        }

        public TImpl CreateRecursively<TImpl>(IEnumerable<Override> overrides = null)
        {
            return (TImpl)Create(typeof(TImpl), overrides);
        }
    }
}