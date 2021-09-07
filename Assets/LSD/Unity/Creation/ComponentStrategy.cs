using System;
using System.Collections.Generic;
using UnityEngine;

namespace LSD.Unity.Creation
{
    public class ComponentStrategy : ICreationalStrategy
    {
        private ISyringe syringe;

        public ComponentStrategy(ISyringe syringe)
        {
            this.syringe = syringe;
        }

        public object Create(Type type, IEnumerable<Override> overrides = null)
        {
            if (!type.IsSubclassOf(typeof(MonoBehaviour)))
                throw new InvalidOperationException($"{type} is not a MonoBehaviour!");

            var instance = new GameObject().AddComponent(type);
            syringe.Inject(instance, overrides);
            return instance;
        }

        public TImpl Create<TImpl>(IEnumerable<Override> overrides = null)
        {
            return (TImpl)Create(typeof(TImpl), overrides);
        }

        public object CreateRecursively(Type type, IEnumerable<Override> overrides = null)
        {
            if (!type.IsSubclassOf(typeof(MonoBehaviour)))
                throw new InvalidOperationException($"{type} is not a MonoBehaviour!");

            var instance = new GameObject().AddComponent(type);
            syringe.InjectRecursively(instance, overrides);
            return instance;
        }

        public TImpl CreateRecursively<TImpl>(IEnumerable<Override> overrides = null)
        {
            return (TImpl)CreateRecursively(typeof(TImpl), overrides);
        }
    }
}