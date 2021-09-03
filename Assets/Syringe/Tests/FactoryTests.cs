using System;
using NUnit.Framework;
using UnityEngine.TestTools;
using Syringe;

public class FactoryTests
{
    [Test]
    public void ResolveConcreteFromNewSingleton() {
        var parent = new DIContainer();

        parent.Register<RandomFactory>().FromNew().AsSingleton();

        var factory = parent.Resolve<RandomFactory>();

        var rnd = factory.Create();

        Assert.IsNotNull(rnd);
    }

    [Test]
    public void ResolveAbstractFromNewSingleton() {
        var parent = new DIContainer();

        parent.Register<IFactory<RandomProvider>, RandomFactory>().FromNew().AsSingleton();

        var factory = parent.Resolve<IFactory<RandomProvider>>();

        var rnd = factory.Create();

        Assert.IsNotNull(rnd);
    }

    [Test]
    public void ResolveCustomAbstractFromNewSingleton() {
        var parent = new DIContainer();

        parent.Register<IRandomFactory, RandomFactory>().FromNew().AsSingleton();

        var factory = parent.Resolve<IRandomFactory>();

        var rnd = factory.Create(Guid.NewGuid());

        Assert.IsNotNull(rnd);
    }

    [Test]
    public void ResolveConcreteFromNewSingletonInDependency() {
        var parent = new UnityDIContainer();
        var container = new UnityDIContainer(parent);

        parent.Register<RandomFactory>().FromNew().AsSingleton();
        container.Register<DependsOnConcreteFactory>().FromNew().AsSingleton();
        container.Register<DependsOnConcreteComponent>().FromNew().AsSingleton();

        var factory = parent.Resolve<RandomFactory>();
        var dep = container.Resolve<DependsOnConcreteFactory>();
        var component = container.Resolve<DependsOnConcreteComponent>();

        Assert.IsNotNull(dep.Random);
    }
}
