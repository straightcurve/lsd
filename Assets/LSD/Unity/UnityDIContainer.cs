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
    }
}
