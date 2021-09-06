using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LSD
{
    public class Syringe : ISyringe
    {
        public DIContainer Container { get; }
        public Syringe (DIContainer container) {
            Container = container;
        }

        public void Inject(object instance, IEnumerable<Override> overrides = null)
        {
            var type = instance.GetType();
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

            if (overrides == null || !overrides.Any(o => o.targetType == type))
                foreach (var field in fields)
                    field.SetValue(instance, Container.Resolve(field.FieldType));
            else 
                foreach (var field in fields) {
                    var _override = overrides.FirstOrDefault(o => o.dependencyType == field.FieldType);
                    if (_override == null)
                        return;

                    field.SetValue(instance, _override.dependency);
                }
        }

        public void InjectRecursively(object instance, IEnumerable<Override> overrides = null)
        {
            var type = instance.GetType();
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

            if (overrides == null || !overrides.Any(o => o.targetType == type))
                foreach (var field in fields)
                    field.SetValue(instance, Container.Resolve(field.FieldType));
            else 
                foreach (var field in fields) {
                    var _override = overrides.FirstOrDefault(o => o.dependencyType == field.FieldType);
                    if (_override == null) {
                        field.SetValue(instance, Container.Resolve(field.FieldType));
                        return;
                    }
    
                    field.SetValue(instance, _override.dependency);
                    InjectRecursively(_override.dependency, overrides);
                }
        }
    }
}