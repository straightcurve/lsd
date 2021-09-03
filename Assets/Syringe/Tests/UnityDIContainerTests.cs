using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Syringe;
using System;

public class UnityDIContainerTests
{
    [Test]
    public void ConcreteSingletonNoDependencyWithInitialValue() {
        var container = new UnityDIContainer();

        container.RegisterSingleton(new GameObject().AddComponent<NoDependencyMono>());

        var rnd1 = container.Resolve<NoDependencyMono>();
        var rnd2 = container.Resolve<NoDependencyMono>();

        Assert.AreEqual(rnd1, rnd2);
    }

    [Test]
    public void ConcreteSingletonNoDependencyNonLazy() {
        var container = new UnityDIContainer();

        container.RegisterSingleton<NoDependencyMono>();

        var rnd1 = container.Resolve<NoDependencyMono>();
        var rnd2 = container.Resolve<NoDependencyMono>();

        Assert.AreEqual(rnd1, rnd2);
    }
    
    [Test]
    public void AbstractSingletonNoDependency() {
        var container = new UnityDIContainer();

        container.RegisterSingleton<INone, NoDependencyMono>();

        var rnd1 = container.Resolve<INone>();
        var rnd2 = container.Resolve<INone>();

        Assert.AreEqual(rnd1, rnd2);
    }

    [Test]
    public void ConcreteTransientNoDependency() {
        var container = new UnityDIContainer();

        container.RegisterTransient<NoDependencyMono>();

        var rnd1 = container.Resolve<NoDependencyMono>();
        var rnd2 = container.Resolve<NoDependencyMono>();

        Assert.AreNotEqual(rnd1, rnd2);
    }

    [Test]
    public void AbstractTransientNoDependency() {
        var container = new UnityDIContainer();

        container.RegisterTransient<INone, NoDependencyMono>();

        var rnd1 = container.Resolve<INone>();
        var rnd2 = container.Resolve<INone>();

        Assert.AreNotEqual(rnd1, rnd2);
    }

    [Test]
    public void SingletonConcreteSingletonDependency() {
        var container = new UnityDIContainer();

        container.RegisterSingleton<RandomProvider>();
        container.RegisterSingleton<ConcreteDependencyMono>();

        var rnd1 = container.Resolve<RandomProvider>();
        var rnd2 = container.Resolve<ConcreteDependencyMono>();

        Assert.IsNotNull(rnd1);
        Assert.IsNotNull(rnd2);
        Assert.AreEqual(rnd1.Value, rnd2.ConcreteValue);
    }

    [Test]
    public void SingletonAbstractSingletonDependency() {
        var container = new UnityDIContainer();

        container.RegisterSingleton<IProvider, RandomProvider>();
        container.RegisterSingleton<AbstractDependencyMono>();

        var rnd1 = container.Resolve<IProvider>();
        var rnd2 = container.Resolve<AbstractDependencyMono>();

        Assert.IsNotNull(rnd1);
        Assert.IsNotNull(rnd2);
        Assert.AreEqual(rnd1.Value, rnd2.AbstractValue);
    }

    [Test]
    public void SingletonConcreteTransientDependency() {
        var container = new UnityDIContainer();

        container.RegisterTransient<RandomProvider>();
        container.RegisterSingleton<ConcreteDependencyMono>();

        var rnd1 = container.Resolve<RandomProvider>();
        var rnd2 = container.Resolve<ConcreteDependencyMono>();

        Assert.IsNotNull(rnd1);
        Assert.IsNotNull(rnd2);
        Assert.AreNotEqual(rnd1.Value, rnd2.ConcreteValue);
    }

    [Test]
    public void SingletonAbstractTransientDependency() {
        var container = new UnityDIContainer();

        container.RegisterTransient<IProvider, RandomProvider>();
        container.RegisterSingleton<AbstractDependencyMono>();

        var rnd1 = container.Resolve<IProvider>();
        var rnd2 = container.Resolve<AbstractDependencyMono>();

        Assert.AreNotEqual(rnd1.Value, rnd2.AbstractValue);
    }
    
    [Test]
    public void TransientConcreteSingletonDependency() {
        var container = new UnityDIContainer();

        container.RegisterSingleton<RandomProvider>();
        container.RegisterTransient<ConcreteDependencyMono>();

        var rnd1 = container.Resolve<ConcreteDependencyMono>();
        var rnd2 = container.Resolve<ConcreteDependencyMono>();

        Assert.AreEqual(rnd1.ConcreteValue, rnd2.ConcreteValue);
        Assert.AreNotEqual(rnd1, rnd2);
    }
    
    [Test]
    public void TransientAbstractSingletonDependency() {
        var container = new UnityDIContainer();

        container.RegisterSingleton<IProvider, RandomProvider>();
        container.RegisterTransient<AbstractDependencyMono>();

        var rnd1 = container.Resolve<AbstractDependencyMono>();
        var rnd2 = container.Resolve<AbstractDependencyMono>();

        Assert.AreEqual(rnd1.AbstractValue, rnd2.AbstractValue);
        Assert.AreNotEqual(rnd1, rnd2);
    }

    [Test]
    public void TransientConcreteTransientDependency() {
        var container = new UnityDIContainer();

        container.RegisterTransient<RandomProvider>();
        container.RegisterTransient<ConcreteDependencyMono>();

        var rnd1 = container.Resolve<ConcreteDependencyMono>();
        var rnd2 = container.Resolve<ConcreteDependencyMono>();

        Assert.AreNotEqual(rnd1.ConcreteValue, rnd2.ConcreteValue);
        Assert.AreNotEqual(rnd1, rnd2);
    }

    [Test]
    public void TransientAbstractTransientDependency() {
        var container = new UnityDIContainer();

        container.RegisterTransient<IProvider, RandomProvider>();
        container.RegisterTransient<AbstractDependencyMono>();

        var rnd1 = container.Resolve<AbstractDependencyMono>();
        var rnd2 = container.Resolve<AbstractDependencyMono>();

        Assert.AreNotEqual(rnd1.AbstractValue, rnd2.AbstractValue);
        Assert.AreNotEqual(rnd1, rnd2);
    }

    [Test]
    public void ResolveConcreteSingletonFromParentContainerWithInitialValue() {
        var parent = new UnityDIContainer();
        var container = new UnityDIContainer(parent);

        parent.RegisterSingleton(new RandomProvider());
        parent.RegisterSingleton<ConcreteDependencyMonoFactory>();

        var factory = parent.Resolve<ConcreteDependencyMonoFactory>();
        container.RegisterSingleton<ConcreteDependencyMono>(factory.Create());

        var rnd1 = parent.Resolve<RandomProvider>();
        var rnd2 = container.Resolve<ConcreteDependencyMono>();

        Assert.AreEqual(rnd1.Value, rnd2.ConcreteValue);
    }

    [Test]
    public void ResolveConcreteSingletonFromParentContainerNonLazy() {
        var parent = new UnityDIContainer();
        var container = new UnityDIContainer(parent);

        parent.RegisterSingleton<RandomProvider>();
        container.RegisterSingleton<ConcreteDependencyMono>();

        var rnd1 = parent.Resolve<RandomProvider>();
        var rnd2 = container.Resolve<ConcreteDependencyMono>();

        Assert.AreEqual(rnd1.Value, rnd2.ConcreteValue);
    }

    [Test]
    public void ResolveAbstractSingletonFromParentContainerNonLazy() {
        var parent = new UnityDIContainer();
        var container = new UnityDIContainer(parent);

        parent.RegisterSingleton<IProvider, RandomProvider>();
        parent.RegisterSingleton<IAbstract, AbstractDependencyMono>();

        var rnd1 = parent.Resolve<IProvider>();
        var rnd2 = container.Resolve<IAbstract>();

        Assert.AreEqual(rnd1.Value, rnd2.AbstractValue);
    }

    [Test]
    public void ResolveAbstractSingletonFromParentContainerWithInitialValue() {
        var parent = new UnityDIContainer();
        var container = new UnityDIContainer(parent);

        parent.RegisterSingleton<IProvider, RandomProvider>(new RandomProvider());
        parent.RegisterSingleton<IFactory<AbstractDependencyMono>, AbstractDependencyMonoFactory>();

        var factory = parent.Resolve<IFactory<AbstractDependencyMono>>();
        container.RegisterSingleton<IAbstract, AbstractDependencyMono>(factory.Create());

        var rnd1 = parent.Resolve<IProvider>();
        var rnd2 = container.Resolve<IAbstract>();

        Assert.AreEqual(rnd1.Value, rnd2.AbstractValue);
    }

    [Test]
    public void ResolveConcreteTransientFromParentContainerNonLazy() {
        var parent = new UnityDIContainer();
        var container = new UnityDIContainer(parent);

        parent.RegisterTransient<RandomProvider>();
        container.RegisterTransient<ConcreteDependencyMono>();

        var rnd1 = parent.Resolve<RandomProvider>();
        var rnd2 = container.Resolve<ConcreteDependencyMono>();

        Assert.AreNotEqual(rnd1.Value, rnd2.ConcreteValue);
    }

    [Test]
    public void ResolveAbstractTransientFromParentContainerNonLazy() {
        var parent = new UnityDIContainer();
        var container = new UnityDIContainer(parent);

        parent.RegisterTransient<IProvider, RandomProvider>();
        container.RegisterTransient<IAbstract, AbstractDependencyMono>();

        var rnd1 = parent.Resolve<IProvider>();
        var rnd2 = container.Resolve<IAbstract>();

        Assert.AreNotEqual(rnd1.Value, rnd2.AbstractValue);
    }

    [Test]
    public void ResolveFromInstanceAsSingletonNonLazy() {
        var container = new UnityDIContainer();

        container.RegisterSingleton<NoDependencyFactory>();

        var factory = container.Resolve<NoDependencyFactory>();
        var guid = Guid.NewGuid();
        var instance = factory.Create();
        instance.Value = guid;

        container.Register<NoDependencyMono>().FromInstance(instance).AsSingleton().NonLazy();

        var resolved = container.Resolve<NoDependencyMono>();

        Assert.AreEqual(instance.Value, resolved.Value);
        Assert.AreEqual(instance, resolved);
    }
}
