using System;
using System.Collections.Generic;
using LSD.Unity.Creation;
using UnityEngine;

namespace LSD.Unity
{
    public interface IUnitySourceSelection<TImpl> : ISourceSelectionStage<TImpl>
    {
        [Obsolete("Use FromNew() instead")]
        ILifetimeSelectionStage FromNewComponent();
        ILifetimeSelectionStage FromPrefab(TImpl prefab);
    }

    public class UnityRegistration<TService, TImpl> : Registration<TService, TImpl>, IUnitySourceSelection<TImpl> where TImpl : MonoBehaviour
    {
        public UnityRegistration(ISyringe syringe) : base(syringe)
        {
            FromNew();
            AsTransient();
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
}