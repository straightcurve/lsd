using LSD;
using UnityEngine;

public class ConcreteDependencyComponent : MonoBehaviour, IConcreteDepComponent {
    [Dependency]
    public NoDependencyComponent Concrete;
}

public interface IConcreteDepComponent {}