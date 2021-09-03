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

        public IUnitySourceSelection<TImpl> RegisterComponent<TImpl>() where TImpl : MonoBehaviour
        {
            return new UnityRegistration<TImpl, TImpl>(this);
        }

        public IUnitySourceSelection<TImpl> RegisterComponent<TService, TImpl>() where TImpl : MonoBehaviour
        {
            return new UnityRegistration<TService, TImpl>(this);
        }

        public class UnityRegistration<TService, TImpl> : Registration<TService, TImpl>, IUnitySourceSelection<TImpl> where TImpl: MonoBehaviour {
            public UnityRegistration(UnityDIContainer container): base(container) {}

            public ILifetimeSelectionStage FromNewComponent() {
                var type = typeof(TImpl);
                Descriptor.GetInstance = () => {
                    var instance = new GameObject().AddComponent(type);
                    Container.Inject(instance);
                    return instance;
                };
                return this;
            }
        }

        public interface IUnitySourceSelection<TImpl> : ISourceSelectionStage<TImpl>
        {
            ILifetimeSelectionStage FromNewComponent();
        }
    }
}
