using LSD;
using UnityEngine;

public class AddressComponent : MonoBehaviour, IAddressComponent
{
    [Dependency]
    private string street;
    public string Street => street;
}

public interface IAddressComponent
{
    string Street { get; }
}