using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LSD.Unity
{
    public class UnityDIContainer : DIContainer
    {

        public UnityDIContainer() : base(null) { }
        public UnityDIContainer(DIContainer _parent) : base(_parent) { }

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

        public class UnityRegistration<TService, TImpl> : Registration<TService, TImpl>, IUnitySourceSelection<TImpl> where TImpl : MonoBehaviour
        {
            public UnityRegistration(ISyringe syringe) : base(syringe)
            {
            }

            public ILifetimeSelectionStage FromNewComponent()
            {
                var type = typeof(TImpl);
                Descriptor.GetInstance = () =>
                {
                    var instance = new GameObject().AddComponent(type);
                    Syringe.Inject(instance);
                    return instance;
                };
                Descriptor.GetOverridenInstance = (IEnumerable<Override> overrides) =>
                {
                    var instance = new GameObject().AddComponent(type);
                    Syringe.Inject(instance, overrides);
                    return instance;
                };
                return this;
            }

            public ILifetimeSelectionStage FromPrefab(TImpl prefab)
            {
                var type = typeof(TImpl);
                Descriptor.GetInstance = () =>
                {
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
