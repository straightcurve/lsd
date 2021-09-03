using System;
using NUnit.Framework;
using UnityEngine.TestTools;
using Syringe;

public class FactoryTests
{
    [Test]
    public void ConcreteFactory() {
        var parent = new DIContainer();

        parent.RegisterSingleton<RandomFactory>();

        var factory = parent.Resolve<RandomFactory>();

        var rnd = factory.Create();

        Assert.IsNotNull(rnd);
    }

    [Test]
    public void AbstractFactory() {
        var parent = new DIContainer();

        parent.RegisterSingleton<IFactory<RandomProvider>, RandomFactory>();

        var factory = parent.Resolve<IFactory<RandomProvider>>();

        var rnd = factory.Create();

        Assert.IsNotNull(rnd);
    }

    [Test]
    public void CustomAbstractFactory() {
        var parent = new DIContainer();

        parent.RegisterSingleton<IRandomFactory, RandomFactory>();

        var factory = parent.Resolve<IRandomFactory>();

        var rnd = factory.Create(Guid.NewGuid());

        Assert.IsNotNull(rnd);
    }

    [Test]
    public void ResolveBaseFactoryInDependency() {
        var parent = new UnityDIContainer();
        var container = new UnityDIContainer(parent);

        parent.RegisterSingleton<RandomFactory>();
        container.RegisterSingleton<DependsOnConcreteFactory>();
        container.RegisterSingleton<DependsOnConcreteComponent>();

        var factory = parent.Resolve<RandomFactory>();
        var dep = container.Resolve<DependsOnConcreteFactory>();
        var component = container.Resolve<DependsOnConcreteComponent>();

        Assert.IsNotNull(dep.Random);
    }
}
