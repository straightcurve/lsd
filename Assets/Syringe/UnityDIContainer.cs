using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Syringe {
    public class UnityDIContainer : DIContainer {

        public UnityDIContainer(): this(null) {}
        public UnityDIContainer(DIContainer _parent) {
            this.parent = _parent;
        }

        public override T Instantiate<T>() {
            return (T)Instantiate(typeof(T));
        }

        public override object Instantiate(Type type) {
            if (type.IsSubclassOf(typeof(Component))) {
                var instance = new GameObject().AddComponent(type);

                var fields = type.GetFields(
                    BindingFlags.Public
                    | BindingFlags.NonPublic
                    | BindingFlags.Instance)
                    .Where(f => f.GetCustomAttributes(typeof(DependencyAttribute), false).Length > 0)
                    .ToArray();
            
                foreach (var field in fields)
                    field.SetValue(instance, Resolve(field.FieldType));

                return instance;
            }

            return base.Instantiate(type);
        }
    }
}
