using System;
using Syringe;
using UnityEngine;

internal class NoDependencyFactory : BaseFactory<NoDependencyMono> {

}

internal interface IRandomFactory : IFactory<RandomProvider> {
    RandomProvider Create(Guid guid);
}

internal class DependsOnConcreteComponent : MonoBehaviour {
    [Dependency]
    private DependsOnConcreteFactory component;

    public RandomProvider Random => component.Random;
}

internal class DependsOnConcreteFactory : MonoBehaviour {
    [Dependency]
    private RandomFactory factory;

    public RandomProvider Random => factory.Create();
}

internal class RandomFactory : BaseFactory<RandomProvider>, IRandomFactory
{
    public RandomProvider Create(Guid guid)
    {
        return container.Instantiate<RandomProvider>();
    }
}

internal class AbstractDependencyMonoFactory : IFactory<AbstractDependencyMono>
{
    [Dependency]
    private DIContainer container;

    public AbstractDependencyMono Create() => container.Instantiate<AbstractDependencyMono>();
}

internal class ConcreteDependencyMonoFactory : IFactory<ConcreteDependencyMono>
{
    [Dependency]
    private DIContainer container;

    public ConcreteDependencyMono Create() => container.Instantiate<ConcreteDependencyMono>();
}

internal class NoDependencyMono : MonoBehaviour, INone {
    public Guid Value { get; set; }
}

internal class ConcreteDependencyMono : MonoBehaviour, IConcrete {

    [Dependency]
    private RandomProvider concrete;

    public Guid ConcreteValue => concrete.Value;
}

internal class AbstractDependencyMono : MonoBehaviour, IAbstract {

    [Dependency]
    private IProvider _abstract;

    public Guid AbstractValue => _abstract.Value;
}

internal class BothDependencyMono : MonoBehaviour, IBoth {

    [Dependency]
    private IProvider _abstract;

    [Dependency]
    private RandomProvider concrete;

    public Guid AbstractValue => _abstract.Value;

    public Guid ConcreteValue => concrete.Value;
}

internal class RandomProvider : IProvider {
    private readonly Guid value = Guid.NewGuid();
    public Guid Value => value;
}

internal interface IProvider {
    Guid Value { get; }
}

internal interface IConcrete {
    Guid ConcreteValue { get; }
}

internal interface IAbstract {
    Guid AbstractValue { get; }
}

internal interface IBoth : IConcrete, IAbstract { }

internal interface INone
{
    Guid Value { get; }
}
