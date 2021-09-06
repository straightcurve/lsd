using NUnit.Framework;
using UnityEngine.TestTools;
using LSD;
using LSD.Builder;
using System;
using LSD.Unity;
using UnityEngine;

public class UnityBuilderTests
{
    [Test]
    public void ResolvesChildDependenciesFromContainer()
    {
        var container = new UnityDIContainer();
        container.Register<string>().FromInstance("221B Baker Street");
        container.RegisterComponent<AddressComponent>().FromNewComponent().AsTransient();
        container.Register<IBuilder<UserComponent>, UnityBuilder<UserComponent>>().FromNew().AsSingleton();

        var builder = container.Resolve<IBuilder<UserComponent>>();
        var user = builder
            .New()
            .Build();

        Assert.IsNotNull(user);
        Assert.IsNotNull(user.Address);
    }

    [Test]
    public void OverridesSpecifiedDependencies()
    {
        var container = new UnityDIContainer();
        container.Register<string>().FromInstance("221B Baker Street");
        container.RegisterComponent<AddressComponent>().FromNewComponent().AsTransient();
        container.Register<IBuilder<UserComponent>, UnityBuilder<UserComponent>>().FromNew().AsSingleton();

        var builder = container.Resolve<IBuilder<UserComponent>>();
        var user = builder
            .New()
            .Override<string, UserComponent>("Sherlock Holmes")
            .Build();

        Assert.IsNotNull(user);
        Assert.IsNotNull(user.Address);
        Assert.AreNotEqual(user.Name, container.Resolve<string>());
        Assert.AreNotEqual(user.Name, user.Address.Street);
    }

    [Test]
    public void ThrowsIfDependenciesCantBeResolved()
    {
        var container = new UnityDIContainer();
        container.Register<IBuilder<UserComponent>, UnityBuilder<UserComponent>>().FromNew().AsSingleton();

        var builder = container.Resolve<IBuilder<UserComponent>>();
        var user = builder
            .New();

        Assert.Throws<NullReferenceException>(() => user.Build());

        user.Override<string, UserComponent>("Sherlock Holmes");
        Assert.Throws<NullReferenceException>(() => user.Build());
    }

    [Test]
    public void ThrowsIfChildDependenciesCantBeResolved()
    {
        var container = new UnityDIContainer();
        container.Register<IBuilder<UserComponent>, UnityBuilder<UserComponent>>().FromNew().AsSingleton();

        var builder = container.Resolve<IBuilder<UserComponent>>();
        var user = builder
            .New()
            .Override<string, UserComponent>("Sherlock Holmes")
            .Override<AddressComponent, UserComponent>(new GameObject().AddComponent<AddressComponent>());

        Assert.Throws<NullReferenceException>(() => user.Build());
    }

    [Test]
    public void ThrowsIfOriginalInstanceIsNull()
    {
        var container = new UnityDIContainer();
        container.Register<IBuilder<UserComponent>, UnityBuilder<UserComponent>>().FromNew().AsSingleton();

        var builder = container.Resolve<IBuilder<UserComponent>>();

        UserComponent random = null;
        Assert.Throws<ArgumentNullException>(() => builder.Clone(random).Build());
    }

    [Test]
    public void ClonesInstance()
    {
        var container = new UnityDIContainer();
        container.Register<IBuilder<UserComponent>, UnityBuilder<UserComponent>>().FromNew().AsSingleton();
        container.Register<string>().FromInstance("placeholder");
        container.RegisterComponent<AddressComponent>().FromNewComponent().AsSingleton();
        container.RegisterComponent<UserComponent>().FromNewComponent().AsSingleton();

        var builder = container.Resolve<IBuilder<UserComponent>>();
        var random = builder
            .New()
            .Build();

        var original = container.Resolve<UserComponent>();

        Assert.AreNotEqual(random, original);

        var cloned = builder.Clone(original).Build();
        Assert.AreNotEqual(cloned, original);
        Assert.AreEqual(cloned.Name, original.Name);
    }

    [Test]
    public void CompilesAndWorks()
    {
        var container = new UnityDIContainer();
        container.Register<IBuilder<UserComponent>, UnityBuilder<UserComponent>>().FromNew().AsSingleton();

        var builder = container.Resolve<IBuilder<UserComponent>>();
        var user1 = builder
            .New()
            .Override<string, UserComponent>("Sherlock Holmes")
            .Override<AddressComponent, UserComponent>(new GameObject().AddComponent<AddressComponent>())
            .Override<string, AddressComponent>("221B Baker Street")
            .Build();
        var user2 = builder
            .New()
            .Override<string, UserComponent>("Sherlock Jolmes")
            .Override<AddressComponent, UserComponent>(new GameObject().AddComponent<AddressComponent>())
            .Override<string, AddressComponent>("223B Baker Street")
            .Build();

        Assert.AreNotEqual(user1, user2);
        Assert.AreNotEqual(user1.Name, user2.Name);
        Assert.AreNotEqual(user1.Address.Street, user2.Address.Street);
    }

    [Test]
    public void CompilesAndWorksDerived()
    {
        var container = new UnityDIContainer();
        container.Register<UserComponent.Builder>().FromNew().AsSingleton();

        var builder = container.Resolve<UserComponent.Builder>();
        var user1 = builder
            .New()
            .Override<string, UserComponent>("Sherlock Holmes")
            .Override<AddressComponent, UserComponent>(new GameObject().AddComponent<AddressComponent>())
            .Override<string, AddressComponent>("221B Baker Street")
            .Build();
        var user2 = builder
            .New()
            .Override<string, UserComponent>("Sherlock Jolmes")
            .Override<AddressComponent, UserComponent>(new GameObject().AddComponent<AddressComponent>())
            .Override<string, AddressComponent>("223B Baker Street")
            .Build();

        Assert.AreNotEqual(user1, user2);
        Assert.AreNotEqual(user1.Name, user2.Name);
        Assert.AreNotEqual(user1.Address.Street, user2.Address.Street);
    }

    [Test]
    public void CreatesNewComponent()
    {
        var container = new UnityDIContainer();
        container.RegisterComponent<NoDependencyComponent>().FromNewComponent().AsSingleton();
        container.Register<IBuilder<NoDependencyComponent>, UnityBuilder<NoDependencyComponent>>().FromNew().AsSingleton();

        var singleton = container.Resolve<NoDependencyComponent>();
        var builder = container.Resolve<IBuilder<NoDependencyComponent>>();
        var built = builder
            .New()
            .Build();

        Assert.AreNotEqual(built, singleton);

        Assert.AreNotEqual(
            builder
            .New()
            .Build(),
            builder
            .New()
            .Build()
        );
    }
}
