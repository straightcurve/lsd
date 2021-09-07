using System;
using UnityEngine;

namespace LSD.Unity
{
    public interface IUnityBuilder<TImpl> : IBuilder<TImpl> where TImpl : MonoBehaviour
    {
        new IUnityBuilder<TImpl> New();
        new IUnityBuilder<TImpl> Clone<TOriginal>(TOriginal instance) where TOriginal : TImpl, ICloneable;
        new IUnityBuilder<TImpl> Override<TDependency, TIn>(TDependency dependency);
        IUnityBuilder<TImpl> Prefab(string path);
        IUnityBuilder<TImpl> Prefab(TImpl prefab);
    }
}