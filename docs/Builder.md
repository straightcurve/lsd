# Builder

Mocks:

```csharp
using LSD;

class Address {
    [Dependency]
    private string street;
    public string Street => street;
}

class User {
    [Dependency]
    private Address address;
    public Address Address => address;

    [Dependency]
    private string name;
    public string Name => name;
}
```

Creating a builder (**MUST** be added to a container):

```csharp
using LSD;

var container = new DIContainer();
container.Register<Builder<User>>().FromNew().AsSingleton();
```

Resolving a builder:

```csharp
var builder = container.Resolve<Builder<User>>();
```

Creating users through constructor (new keyword):

```csharp
container.Register<Address>().FromNew().AsSingleton();
container.Register<string>().FromInstance("221B Baker Street");

var user = builder.New()
    .Override<string, User>("Sherlock Holmes")
    .Build();

Result:
    user = {
        name: "Sherlock Holmes",
        address: {
            street: "221B Baker Street"
        }
    }
```

Creating users by cloning an already created instance:

```csharp
/** from above

    container.Register<Address>().FromNew().AsSingleton();
    container.Register<string>().FromInstance("221B Baker Street");
 */

var original = builder.New()
    .Override<string, User>("Sherlock Holmes")
    .Build();

var clone = builder.Clone(original)
    .Build();

Assert.AreEqual(original.Name, clone.Name);
Assert.AreNotEqual(original, clone);

Result:
    original = {
        name: "Sherlock Holmes",
        address: {
            street: "221B Baker Street"
        }
    }

    clone = {
        name: "Sherlock Holmes",
        address: {
            street: "221B Baker Street"
        }
    }

    pass
    pass
```

Override dependencies inside children:

```csharp
/** from above

    container.Register<Address>().FromNew().AsSingleton();
    container.Register<string>().FromInstance("221B Baker Street");
 */

var user = builder.New()
    .Override<string, User>("Sweet Acid")
    .Override<string, Address>("2B Programmer Street")
    .Build();

Result:
    user = {
        name: "Sweet Acid",
        address: {
            street: "2B Programmer Street"
        }
    }
```
