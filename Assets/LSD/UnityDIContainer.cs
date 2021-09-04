using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace LSD {
    public class UnityDIContainer : DIContainer {

        public UnityDIContainer(): base(null) {}
        public UnityDIContainer(DIContainer _parent) : base(_parent) {}

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
            return RegisterComponent<TImpl, TImpl>();
        }

        public IUnitySourceSelection<TImpl> RegisterComponent<TService, TImpl>() where TImpl : MonoBehaviour
        {
            var reg = new UnityRegistration<TService, TImpl>(syringe);
            collection.Add(typeof(TService), reg.Descriptor);
            return reg;
        }

        public class UnityRegistration<TService, TImpl> : Registration<TService, TImpl>, IUnitySourceSelection<TImpl> where TImpl: MonoBehaviour {
            public UnityRegistration(ISyringe syringe) : base(syringe)
            {
            }

            public ILifetimeSelectionStage FromNewComponent() {
                var type = typeof(TImpl);
                Descriptor.GetInstance = () => {
                    var instance = new GameObject().AddComponent(type);
                    Syringe.Inject(instance);
                    return instance;
                };
                return this;
            }

            public ILifetimeSelectionStage FromPrefab(TImpl prefab) {
                var type = typeof(TImpl);
                Descriptor.GetInstance = () => {
                    var go = GameObject.Instantiate(prefab.gameObject);
                    go.GetComponents<Component>().ToList().ForEach((c) => Syringe.Inject(c));
                    return go.GetComponent<TImpl>();
                };
                return this;
            }
        }

        public interface IUnitySourceSelection<TImpl> : ISourceSelectionStage<TImpl>
        {
            ILifetimeSelectionStage FromNewComponent();
            ILifetimeSelectionStage FromPrefab(TImpl prefab);
        }
    }
}
