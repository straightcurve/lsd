using System;
using UnityEngine;
using LSD.Unity.Creation;

namespace LSD.Unity
{
    public class UnityBuilder<TImpl> : Builder<TImpl>, IUnityBuilder<TImpl> where TImpl : MonoBehaviour
    {
        public new IUnityBuilder<TImpl> Override<TDependency, TIn>(TDependency dependency)
        {
            return (IUnityBuilder<TImpl>)base.Override<TDependency, TIn>(dependency);
        }

        public new IUnityBuilder<TImpl> New()
        {
            strategy = new ComponentStrategy(syringe);
            return this;
        }

        public new IUnityBuilder<TImpl> Clone<TOriginal>(TOriginal instance) where TOriginal : TImpl, ICloneable
        {
            if (instance == null) throw new ArgumentNullException("instance");

            strategy = new CloneComponentStrategy(instance);
            return this;
        }

        public IUnityBuilder<TImpl> Prefab(string path)
        {
            if (path == null) throw new ArgumentNullException("path");

            strategy = new PrefabStrategy(syringe, path, typeof(TImpl));
            return this;
        }

        public IUnityBuilder<TImpl> Prefab(TImpl prefab)
        {
            if (prefab == null)
                throw new ArgumentNullException("prefab");

            strategy = new PrefabStrategy(syringe, prefab);
            return this;
        }
    }
}