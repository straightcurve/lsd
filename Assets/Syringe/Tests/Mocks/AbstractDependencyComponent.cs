using UnityEngine;
using Syringe;

public class AbstractDependencyComponent : MonoBehaviour, IAbstractDepComponent {
    [Dependency]
    public NoDependencyComponent Abstract;
}

public interface IAbstractDepComponent {}