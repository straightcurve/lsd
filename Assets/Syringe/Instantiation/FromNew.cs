using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Syringe {
    public class FromNewStrategy<TInstance>
    {
        public DIContainer Container { get; }

        public FromNewStrategy(DIContainer container) {
            Container = container;
        }

        public object Instantiate()
        {
            var instance = Activator.CreateInstance<TInstance>();

            var type = typeof(TInstance);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(f => f.GetCustomAttributes(typeof(DependencyAttribute), false).Length > 0)
                .ToList();

            var derivedType = type.BaseType;
            while (derivedType != null) {
                derivedType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(f => f.GetCustomAttributes(typeof(DependencyAttribute), false).Length > 0)
                    .ToList().ForEach(f => fields.Add(f));
                derivedType = derivedType.BaseType;
            }

            foreach (var field in fields)
                field.SetValue(instance, Container.Resolve(field.FieldType));

            return instance;
        }
    }
}
