using UnityEngine;
using LSD;

public class AbstractDependencyComponent : MonoBehaviour, IAbstractDepComponent {
    [Dependency]
    public NoDependencyComponent Abstract;
}

public interface IAbstractDepComponent {}