using System;
using LSD;
using LSD.Unity;
using UnityEngine;

public class UserComponent : MonoBehaviour, IUserComponent, ICloneable
{
    [Dependency]
    private AddressComponent address;
    public AddressComponent Address => address;

    [Dependency]
    private new string name;
    public string Name => name;

    internal class Builder : UnityBuilder<UserComponent> { }

    public object Clone()
    {
        return Instantiate(this);
    }
}

public interface IUserComponent
{
    AddressComponent Address { get; }
    string Name { get; }
}