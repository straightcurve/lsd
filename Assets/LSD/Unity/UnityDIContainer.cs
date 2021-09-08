using System;
using System.Collections.Generic;
using System.Linq;
using LSD.Unity.Creation;
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

            public new ILifetimeSelectionStage FromNew()
            {
                var type = typeof(TImpl);
                if (!type.IsSubclassOf(typeof(MonoBehaviour)))
                    return base.FromNew();

                strategy = new ComponentStrategy(Syringe);

                Descriptor.GetInstance = () => strategy.Create(type);
                Descriptor.GetOverridenInstance = (IEnumerable<Override> overrides) => strategy.Create(type, overrides);

                return this;
            }

            public ILifetimeSelectionStage FromNewComponent()
            {
                return FromNew();
            }

            public ILifetimeSelectionStage FromPrefab(TImpl prefab)
            {
                var type = typeof(TImpl);
                strategy = new PrefabStrategy(Syringe, prefab);
                Descriptor.GetInstance = () => strategy.Create(type);
                return this;
            }
        }

        public interface IUnitySourceSelection<TImpl> : ISourceSelectionStage<TImpl>
        {
            [Obsolete("Use FromNew() instead")]
            ILifetimeSelectionStage FromNewComponent();
            ILifetimeSelectionStage FromPrefab(TImpl prefab);
        }
    }
}
