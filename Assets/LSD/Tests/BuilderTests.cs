using NUnit.Framework;
using UnityEngine.TestTools;
using LSD;
using System;

public class BuilderTests
{
    [Test]
    public void ResolvesChildDependenciesFromContainer()
    {
        var container = new DIContainer();
        container.Register<string>().FromInstance("221B Baker Street");
        container.Register<Address>().FromNew().AsTransient();
        container.Register<IBuilder<User>, Builder<User>>().FromNew().AsSingleton();

        var builder = container.Resolve<IBuilder<User>>();
        var user = builder
            .New()
            .Build();

        Assert.IsNotNull(user);
        Assert.IsNotNull(user.Address);
    }

    [Test]
    public void OverridesSpecifiedDependencies()
    {
        var container = new DIContainer();
        container.Register<string>().FromInstance("221B Baker Street");
        container.Register<Address>().FromNew().AsTransient();
        container.Register<IBuilder<User>, Builder<User>>().FromNew().AsSingleton();

        var builder = container.Resolve<IBuilder<User>>();
        var user = builder
            .New()
            .Override<string, User>("Sherlock Holmes")
            .Build();

        Assert.IsNotNull(user);
        Assert.IsNotNull(user.Address);
        Assert.AreNotEqual(user.Name, container.Resolve<string>());
        Assert.AreNotEqual(user.Name, user.Address.Street);
    }

    [Test]
    public void ThrowsIfDependenciesCantBeResolved()
    {
        var container = new DIContainer();
        container.Register<IBuilder<User>, Builder<User>>().FromNew().AsSingleton();

        var builder = container.Resolve<IBuilder<User>>();
        var user = builder
            .New();

        Assert.Throws<NullReferenceException>(() => user.Build());

        user.Override<string, User>("Sherlock Holmes");
        Assert.Throws<NullReferenceException>(() => user.Build());
    }

    [Test]
    public void ThrowsIfChildDependenciesCantBeResolved()
    {
        var container = new DIContainer();
        container.Register<IBuilder<User>, Builder<User>>().FromNew().AsSingleton();

        var builder = container.Resolve<IBuilder<User>>();
        var user = builder
            .New()
            .Override<string, User>("Sherlock Holmes")
            .Override<Address, User>(new Address());

        Assert.Throws<NullReferenceException>(() => user.Build());
    }

    [Test]
    public void CreatesInstanceUsingConstructor()
    {
        var container = new DIContainer();
        container.Register<IBuilder<RandomProvider>, Builder<RandomProvider>>().FromNew().AsSingleton();
        container.Register<RandomProvider>().FromNew().AsSingleton();

        var builder = container.Resolve<IBuilder<RandomProvider>>();
        var random = builder
            .New()
            .Build();

        Assert.AreNotEqual(random, container.Resolve<RandomProvider>());

        Assert.AreNotEqual(
            builder
            .New()
            .Build(),
            builder
            .New()
            .Build()
        );
    }

    [Test]
    public void ThrowsIfOriginalInstanceIsNull()
    {
        var container = new DIContainer();
        container.Register<IBuilder<RandomProvider>, Builder<RandomProvider>>().FromNew().AsSingleton();

        var builder = container.Resolve<IBuilder<RandomProvider>>();

        RandomProvider random = null;
        Assert.Throws<ArgumentNullException>(() => builder.Clone(random).Build());
    }

    [Test]
    public void ClonesInstance()
    {
        var container = new DIContainer();
        container.Register<IBuilder<RandomProvider>, Builder<RandomProvider>>().FromNew().AsSingleton();
        container.Register<RandomProvider>().FromNew().AsSingleton();

        var builder = container.Resolve<IBuilder<RandomProvider>>();
        var random = builder
            .New()
            .Build();

        var original = container.Resolve<RandomProvider>();

        Assert.AreNotEqual(random, original);

        var cloned = builder.Clone(original).Build();
        Assert.AreNotEqual(cloned, original);
        Assert.AreEqual(original.Value, cloned.Value);
    }

    [Test]
    public void CompilesAndWorks()
    {
        var container = new DIContainer();
        container.Register<IBuilder<User>, Builder<User>>().FromNew().AsSingleton();

        var builder = container.Resolve<IBuilder<User>>();
        var user1 = builder
            .New()
            .Override<string, User>("Sherlock Holmes")
            .Override<Address, User>(new Address())
            .Override<string, Address>("221B Baker Street")
            .Build();
        var user2 = builder
            .New()
            .Override<string, User>("Sherlock Jolmes")
            .Override<Address, User>(new Address())
            .Override<string, Address>("223B Baker Street")
            .Build();

        Assert.AreNotEqual(user1, user2);
        Assert.AreNotEqual(user1.Name, user2.Name);
        Assert.AreNotEqual(user1.Address.Street, user2.Address.Street);
    }

    [Test]
    public void CompilesAndWorksDerived()
    {
        var container = new DIContainer();
        container.Register<User.Builder>().FromNew().AsSingleton();

        var builder = container.Resolve<User.Builder>();
        var user1 = builder
            .New()
            .Override<string, User>("Sherlock Holmes")
            .Override<Address, User>(new Address())
            .Override<string, Address>("221B Baker Street")
            .Build();
        var user2 = builder
            .New()
            .Override<string, User>("Sherlock Jolmes")
            .Override<Address, User>(new Address())
            .Override<string, Address>("223B Baker Street")
            .Build();

        Assert.AreNotEqual(user1, user2);
        Assert.AreNotEqual(user1.Name, user2.Name);
        Assert.AreNotEqual(user1.Address.Street, user2.Address.Street);
    }
}
