using System;
using LSD;
using LSD.Builder;
using UnityEngine;

internal class Address
{
    [Dependency]
    private string street;
    public string Street => street;
}

internal class User
{
    [Dependency]
    private string name;
    public string Name => name;

    [Dependency]
    private Address address;
    public Address Address => address;

    internal class Builder : Builder<User>
    {

    }
}

internal class NoDependencyMono : MonoBehaviour, INone
{
    public Guid Value { get; set; }
}

internal class ConcreteDependencyMono : MonoBehaviour, IConcrete
{

    [Dependency]
    private RandomProvider concrete;

    public Guid ConcreteValue => concrete.Value;
}

internal class AbstractDependencyMono : MonoBehaviour, IAbstract
{

    [Dependency]
    private IProvider _abstract;

    public Guid AbstractValue => _abstract.Value;
}

internal class BothDependencyMono : MonoBehaviour, IBoth
{

    [Dependency]
    private IProvider _abstract;

    [Dependency]
    private RandomProvider concrete;

    public Guid AbstractValue => _abstract.Value;

    public Guid ConcreteValue => concrete.Value;
}

internal class RandomProvider : IProvider, ICloneable
{
    private readonly Guid value = Guid.NewGuid();
    public Guid Value => value;

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}

internal interface IProvider
{
    Guid Value { get; }
}

internal interface IConcrete
{
    Guid ConcreteValue { get; }
}

internal interface IAbstract
{
    Guid AbstractValue { get; }
}

internal interface IBoth : IConcrete, IAbstract { }

internal interface INone
{
    Guid Value { get; }
}
