using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LSD.Unity.Creation
{
    public class PrefabStrategy : ICreationalStrategy
    {
        private ISyringe syringe;
        private GameObject prefab;
        private Type type;

        public PrefabStrategy(ISyringe syringe, string path, Type type)
        {
            this.syringe = syringe;
            this.prefab = (GameObject)Resources.Load(path);
            if (this.prefab == null)
                throw new InvalidOperationException($"Invalid prefab path, stat {path}");
            this.type = type;
        }

        public PrefabStrategy(ISyringe syringe, MonoBehaviour prefab)
        {
            this.syringe = syringe;
            this.prefab = prefab.gameObject;
            this.type = prefab.GetType();
        }

        public object Create(Type type, IEnumerable<Override> overrides = null)
        {
            if (!type.IsSubclassOf(typeof(MonoBehaviour)))
                throw new InvalidOperationException($"{type} is not a MonoBehaviour!");

            var instance = GameObject.Instantiate(prefab).GetComponent(type);
            instance.GetComponents<Component>().ToList().ForEach((c) => syringe.Inject(c, overrides));
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

            var instance = GameObject.Instantiate(prefab).GetComponent(type);
            instance.GetComponents<MonoBehaviour>().ToList().ForEach((c) => syringe.InjectRecursively(c, overrides));
            return instance;
        }

        public TImpl CreateRecursively<TImpl>(IEnumerable<Override> overrides = null)
        {
            return ((TImpl)CreateRecursively(typeof(TImpl), overrides));
        }
    }
}