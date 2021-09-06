using NUnit.Framework;
using UnityEngine.TestTools;
using LSD;
using LSD.Builder;
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
            .FromNew()
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
            .FromNew()
            .Override<string, User>("Sherlock Holmes")
            .Build();

        Assert.IsNotNull(user);
        Assert.IsNotNull(user.Address);
        Assert.AreNotEqual(user.Name, container.Resolve<string>());
        Assert.AreNotEqual(user.Name, user.Address.Street);
    }

    [Test]
    public void ThrowsDependenciesCantBeResolved()
    {
        var container = new DIContainer();
        container.Register<IBuilder<User>, Builder<User>>().FromNew().AsSingleton();

        var builder = container.Resolve<IBuilder<User>>();
        var user = builder
            .FromNew();

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
            .FromNew()
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
            .FromNew()
            .Build();

        Assert.AreNotEqual(random, container.Resolve<RandomProvider>());

        Assert.AreNotEqual(
            builder
            .FromNew()
            .Build(),
            builder
            .FromNew()
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
            .FromNew()
            .Build();

        var original = container.Resolve<RandomProvider>();

        Assert.AreNotEqual(random, original);

        var cloned = builder.Clone(original).Build();
        Assert.AreNotEqual(cloned, original);
        Assert.AreEqual(cloned.Value, original.Value);
    }

    [Test]
    public void CompilesAndWorks()
    {
        var container = new DIContainer();
        container.Register<IBuilder<User>, Builder<User>>().FromNew().AsSingleton();

        var builder = container.Resolve<IBuilder<User>>();
        var user1 = builder
            .FromNew()
            .Override<string, User>("Sherlock Holmes")
            .Override<Address, User>(new Address())
            .Override<string, Address>("221B Baker Street")
            .Build();
        var user2 = builder
            .FromNew()
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
            .FromNew()
            .Override<string, User>("Sherlock Holmes")
            .Override<Address, User>(new Address())
            .Override<string, Address>("221B Baker Street")
            .Build();
        var user2 = builder
            .FromNew()
            .Override<string, User>("Sherlock Jolmes")
            .Override<Address, User>(new Address())
            .Override<string, Address>("223B Baker Street")
            .Build();

        Assert.AreNotEqual(user1, user2);
        Assert.AreNotEqual(user1.Name, user2.Name);
        Assert.AreNotEqual(user1.Address.Street, user2.Address.Street);
    }
}
