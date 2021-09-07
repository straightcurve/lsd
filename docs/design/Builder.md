# Builder

We have these User and Address classes with User having a hard dependency on Address.

```csharp
class Address {
    [Dependency]
    private string street;
    public string Street => street;
}

class User {
    [Dependency]
    private string name;
    public string Name => name;

    [Dependency]
    private Address address;
    public Address Address => address;
}
```

So how do we create different Users without making it too hard for _everyone_?

For now, let's go with the Builder pattern.

Clients should be able to do something along those lines.

```csharp
class UserBuilder : Builder<User> { }

public void SomeMethodSomewhere() {
    var builder = new UserBuilder();
    for (int i = 1; i < 10; i++) {
        builder
            .FromNew()
            .With($"Sherlock {'G' + i}olmes")
            .In<User>("name")
            .With($"22{i}B Baker Street")
            .In<Address>("street")
            .Build();
    }
}
```

We can implement `With<TDependency>(TDependency value)`, but unfortunately, this approach doesn't work.
`TBuilder In<TIn>(string name)` is supposed to return the type of builder the user creates, however that wouldn't be possible since we need the instance which we can't get (in a clean way).

```csharp
public abstract class Builder<TImpl, TBuilder> : IBuilder<TImpl, TBuilder> {

    public virtual TBuilder In<TIn>(string name) {
        // do stuff..

        // doesn't compile, Builder is not of type TBuilder
        // note that we can't add a generic class constraint
        // like `where TBuilder : Builder<?>`; for hopefully
        // obvious reasons
        return (TBuilder)this;

        return toSearchingForACleanSolution;
    }

    // ... other methods have the same problem...
}
```

On to the next one.

So let's consider this abstraction:

```csharp
public interface IBuilder<TImpl> {
    TImpl Build();
    IBuilder<TImpl> New();
    IBuilder<TImpl> Override<TDependency, TIn>(TDependency dependency);
}
```

This would let use write something like:

```csharp
public void SomeMethodSomewhereElse() {
    var builder = new Builder<User>();
    for (int i = 1; i < 10; i++) {
        builder
            .New()
            .Override<string, User>($"Sherlock {'G' + i}olmes")
            .Override<Address, User>(new Address())
            .Override<string, Address>($"22{i}B Baker Street")
            .Build();
    }
}
```

It would be nice if we didn't have to ```new Address()``` explicitly but that can't be helped for now.

However as you can see, **we can now easily override child dependencies.**

![YAY!](../assets/excited.gif)

Combining the ```With()``` and ```In()``` methods seems to have fixed the issue we had earlier, but why?
It's because we don't rely on the client derived class anymore.

Say you wanted to build a house. You would hire someone to do it and you would expect them to come with their own tools, right?
Well.. our first implementation meant that they would ask you for tools instead and all they could do is build.

Unless we decided to go dirty and have the clients pass in the instance of the ```TBuilder``` they created through a set method, they would have instead had to implement the ```With()``` and ```In()``` methods themselves in every derived class.

![Yikes](../assets/yikes.gif)

Here we treat the ```Builder``` class as an actual builder, the builder should come with their own tools, not ask you if you have any.

So yeah, this design should be good enough for now. I will update this if requirements change.
