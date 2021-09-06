using System;
using LSD.CreationalStrategies;
using LSD.Builder;
using UnityEngine;
using LSD.Unity.Creation;

namespace LSD.Unity
{
    public class UnityBuilder<TImpl> : Builder<TImpl> where TImpl : MonoBehaviour
    {
        public override IBuilder<TImpl> New()
        {
            strategy = new ComponentStrategy(syringe);
            return this;
        }

        public override IBuilder<TImpl> Clone<TOriginal>(TOriginal instance)
        {
            if (instance == null) throw new ArgumentNullException("instance");

            strategy = new CloneComponentStrategy(instance);
            return this;
        }
    }
}