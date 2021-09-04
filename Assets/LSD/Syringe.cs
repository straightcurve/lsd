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

        public virtual void Inject(object instance, Dictionary<string, object> overrides = null) {
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

            if (overrides == null)
                foreach (var field in fields)
                    field.SetValue(instance, Container.Resolve(field.FieldType));
            else 
                foreach (var field in fields) {
                    if (!overrides.ContainsKey(field.Name))
                        return;

                    field.SetValue(instance, Container.Resolve(field.FieldType));
                }
        }
    }
}