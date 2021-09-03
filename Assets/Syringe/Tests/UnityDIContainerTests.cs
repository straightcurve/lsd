using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Syringe;
using System;

public class UnityDIContainerTests
{
    [Test]
    public void ResolveConcreteFromNewComponentSingleton() {
        var container = new UnityDIContainer();

        container.RegisterComponent<NoDependencyMono>().FromNewComponent().AsSingleton();

        var rnd1 = container.Resolve<NoDependencyMono>();
        var rnd2 = container.Resolve<NoDependencyMono>();

        Assert.AreEqual(rnd1.Value, rnd2.Value);
    }
    
    [Test]
    public void ResolveAbstractFromNewComponentSingleton() {
        var container = new UnityDIContainer();

        container.RegisterComponent<INone, NoDependencyMono>().FromNewComponent().AsSingleton();

        var rnd1 = container.Resolve<INone>();
        var rnd2 = container.Resolve<INone>();

        Assert.AreEqual(rnd1.Value, rnd2.Value);
    }

    [Test]
    public void ResolveConcreteFromInstance() {
        var container = new UnityDIContainer();

        container.RegisterComponent<NoDependencyMono>().FromInstance(new GameObject().AddComponent<NoDependencyMono>());

        var rnd1 = container.Resolve<NoDependencyMono>();
        var rnd2 = container.Resolve<NoDependencyMono>();

        Assert.AreEqual(rnd1.Value, rnd2.Value);
        Assert.AreEqual(rnd1, rnd2);
    }

    [Test]
    public void ResolveAbstractFromInstance() {
        var container = new UnityDIContainer();

        container.RegisterComponent<INone, NoDependencyMono>().FromInstance(new GameObject().AddComponent<NoDependencyMono>());

        var rnd1 = container.Resolve<INone>();
        var rnd2 = container.Resolve<INone>();

        Assert.AreEqual(rnd1, rnd2);
    }

    [Test]
    public void ResolveConcreteFromNewComponentTransient() {
        var container = new UnityDIContainer();

        container.RegisterComponent<NoDependencyMono>().FromNewComponent().AsTransient();

        var rnd1 = container.Resolve<NoDependencyMono>();
        var rnd2 = container.Resolve<NoDependencyMono>();

        Assert.AreNotEqual(rnd1, rnd2);
    }

    [Test]
    public void ResolveAbstractFromNewComponentTransient() {
        var container = new UnityDIContainer();

        container.RegisterComponent<INone, NoDependencyMono>().FromNewComponent().AsTransient();

        var rnd1 = container.Resolve<INone>();
        var rnd2 = container.Resolve<INone>();

        Assert.AreNotEqual(rnd1, rnd2);
    }

    [Test]
    public void ResolveConcreteFromNewSingletonForFromNewComponentSingleton() {
        var container = new UnityDIContainer();

        container.Register<RandomProvider>().FromNew().AsSingleton();
        container.RegisterComponent<ConcreteDependencyMono>().FromNewComponent().AsSingleton();

        var rnd1 = container.Resolve<RandomProvider>();
        var rnd2 = container.Resolve<ConcreteDependencyMono>();

        Assert.AreEqual(rnd1.Value, rnd2.ConcreteValue);
    }

    [Test]
    public void ResolveAbstractFromNewSingletonForFromNewComponentSingleton() {
        var container = new UnityDIContainer();

        container.Register<IProvider, RandomProvider>().FromNew().AsSingleton();
        container.RegisterComponent<AbstractDependencyMono>().FromNewComponent().AsSingleton();

        var rnd1 = container.Resolve<IProvider>();
        var rnd2 = container.Resolve<AbstractDependencyMono>();

        Assert.IsNotNull(rnd1);
        Assert.IsNotNull(rnd2);
        Assert.AreEqual(rnd1.Value, rnd2.AbstractValue);
    }

    [Test]
    public void ResolveConcreteFromNewTransientForFromNewComponentSingleton() {
        var container = new UnityDIContainer();

        container.Register<RandomProvider>().FromNew().AsTransient();
        container.RegisterComponent<ConcreteDependencyMono>().FromNewComponent().AsSingleton();

        var rnd1 = container.Resolve<RandomProvider>();
        var rnd2 = container.Resolve<ConcreteDependencyMono>();

        Assert.IsNotNull(rnd1);
        Assert.IsNotNull(rnd2);
        Assert.AreNotEqual(rnd1.Value, rnd2.ConcreteValue);
    }

    [Test]
    public void ResolveAbstractFromNewTransientForFromNewComponentSingleton() {
        var container = new UnityDIContainer();

        container.Register<IProvider, RandomProvider>().FromNew().AsTransient();
        container.RegisterComponent<AbstractDependencyMono>().FromNewComponent().AsSingleton();

        var rnd1 = container.Resolve<IProvider>();
        var rnd2 = container.Resolve<AbstractDependencyMono>();

        Assert.AreNotEqual(rnd1.Value, rnd2.AbstractValue);
    }
    
    [Test]
    public void ResolveConcreteFromNewSingletonForFromNewComponentTransient() {
        var container = new UnityDIContainer();

        container.Register<RandomProvider>().FromNew().AsSingleton();
        container.RegisterComponent<ConcreteDependencyMono>().FromNewComponent().AsTransient();

        var rnd1 = container.Resolve<ConcreteDependencyMono>();
        var rnd2 = container.Resolve<ConcreteDependencyMono>();

        Assert.AreEqual(rnd1.ConcreteValue, rnd2.ConcreteValue);
        Assert.AreNotEqual(rnd1, rnd2);
    }
    
    [Test]
    public void ResolveAbstractFromNewSingletonForFromNewComponentTransient() {
        var container = new UnityDIContainer();

        container.Register<IProvider, RandomProvider>().FromNew().AsSingleton();
        container.RegisterComponent<AbstractDependencyMono>().FromNewComponent().AsTransient();

        var rnd1 = container.Resolve<AbstractDependencyMono>();
        var rnd2 = container.Resolve<AbstractDependencyMono>();

        Assert.AreEqual(rnd1.AbstractValue, rnd2.AbstractValue);
        Assert.AreNotEqual(rnd1, rnd2);
    }

    [Test]
    public void ResolveConcreteFromNewTransientForFromNewComponentTransient() {
        var container = new UnityDIContainer();

        container.Register<RandomProvider>().FromNew().AsTransient();
        container.RegisterComponent<ConcreteDependencyMono>().FromNewComponent().AsTransient();

        var rnd1 = container.Resolve<ConcreteDependencyMono>();
        var rnd2 = container.Resolve<ConcreteDependencyMono>();

        Assert.AreNotEqual(rnd1.ConcreteValue, rnd2.ConcreteValue);
        Assert.AreNotEqual(rnd1, rnd2);
    }

    [Test]
    public void ResolveAbstractFromNewTransientForFromNewComponentTransient() {
        var container = new UnityDIContainer();

        container.Register<IProvider, RandomProvider>().FromNew().AsTransient();
        container.RegisterComponent<AbstractDependencyMono>().FromNewComponent().AsTransient();

        var rnd1 = container.Resolve<AbstractDependencyMono>();
        var rnd2 = container.Resolve<AbstractDependencyMono>();

        Assert.AreNotEqual(rnd1.AbstractValue, rnd2.AbstractValue);
        Assert.AreNotEqual(rnd1, rnd2);
    }

    [Test]
    public void ResolveConcreteFromInstanceFromParentContainer() {
        var parent = new UnityDIContainer();
        var container = new UnityDIContainer(parent);

        parent.Register<RandomProvider>().FromInstance(new RandomProvider());

        var rnd1 = parent.Resolve<RandomProvider>();
        var rnd2 = container.Resolve<RandomProvider>();

        Assert.AreEqual(rnd1.Value, rnd2.Value);
    }

    /// <summary>
    /// TODO: Change these to components
    /// </summary>

    [Test]
    public void ResolveConcreteFromNewComponentSingletonFromParentContainer() {
        var parent = new UnityDIContainer();
        var container = new UnityDIContainer(parent);

        parent.Register<RandomProvider>().FromNew().AsSingleton();

        var rnd1 = parent.Resolve<RandomProvider>();
        var rnd2 = container.Resolve<RandomProvider>();

        Assert.AreEqual(rnd1.Value, rnd2.Value);
    }

    [Test]
    public void ResolveAbstractFromNewComponentSingletonFromParentContainer() {
        var parent = new UnityDIContainer();
        var container = new UnityDIContainer(parent);

        parent.Register<IProvider, RandomProvider>().FromNew().AsSingleton();

        var rnd1 = parent.Resolve<IProvider>();
        var rnd2 = container.Resolve<IProvider>();

        Assert.AreEqual(rnd1.Value, rnd2.Value);
    }

    [Test]
    public void ResolveAbstractFromInstanceFromParentContainer() {
        var parent = new UnityDIContainer();
        var container = new UnityDIContainer(parent);

        parent.Register<IProvider, RandomProvider>().FromInstance(new RandomProvider());

        var rnd1 = parent.Resolve<IProvider>();
        var rnd2 = container.Resolve<IProvider>();

        Assert.AreEqual(rnd1.Value, rnd2.Value);
    }

    [Test]
    public void ResolveConcreteFromNewComponentTransientFromParentContainer() {
        var parent = new UnityDIContainer();
        var container = new UnityDIContainer(parent);

        parent.Register<RandomProvider>().FromNew().AsTransient();

        var rnd1 = parent.Resolve<RandomProvider>();
        var rnd2 = container.Resolve<RandomProvider>();

        Assert.AreNotEqual(rnd1.Value, rnd2.Value);
    }

    [Test]
    public void ResolveAbstractFromNewComponentTransientFromParentContainer() {
        var parent = new UnityDIContainer();
        var container = new UnityDIContainer(parent);

        parent.Register<IProvider, RandomProvider>().FromNew().AsTransient();

        var rnd1 = parent.Resolve<IProvider>();
        var rnd2 = container.Resolve<IProvider>();

        Assert.AreNotEqual(rnd1.Value, rnd2.Value);
    }

    [Test]
    public void ResolveConcreteFromPrefabSingleton() {
        var container = new UnityDIContainer();

        container.RegisterComponent<NoDependencyComponent>().FromPrefab(Resources.Load<NoDependencyComponent>("none")).AsSingleton();

        var resolved1 = container.Resolve<NoDependencyComponent>();
        var resolved2 = container.Resolve<NoDependencyComponent>();

        Assert.IsNotNull(resolved1);
        Assert.AreEqual(resolved1, resolved2);
    }

    [Test]
    public void ResolveConcreteFromPrefabTransient() {
        var container = new UnityDIContainer();

        container.RegisterComponent<NoDependencyComponent>().FromPrefab(Resources.Load<NoDependencyComponent>("none")).AsTransient();

        var resolved1 = container.Resolve<NoDependencyComponent>();
        var resolved2 = container.Resolve<NoDependencyComponent>();

        Assert.IsNotNull(resolved1);
        Assert.IsNotNull(resolved2);
        Assert.AreNotEqual(resolved1, resolved2);
    }

    [Test]
    public void ResolveAbstractFromPrefabSingleton() {
        var container = new UnityDIContainer();

        container.RegisterComponent<INoDepComponent, NoDependencyComponent>().FromPrefab(Resources.Load<NoDependencyComponent>("none")).AsSingleton();

        var resolved1 = container.Resolve<INoDepComponent>();
        var resolved2 = container.Resolve<INoDepComponent>();

        Assert.IsNotNull(resolved1);
        Assert.AreEqual(resolved1, resolved2);
    }

    [Test]
    public void ResolveAbstractFromPrefabTransient() {
        var container = new UnityDIContainer();

        container.RegisterComponent<INoDepComponent, NoDependencyComponent>().FromPrefab(Resources.Load<NoDependencyComponent>("none")).AsTransient();

        var resolved1 = container.Resolve<INoDepComponent>();
        var resolved2 = container.Resolve<INoDepComponent>();

        Assert.IsNotNull(resolved1);
        Assert.IsNotNull(resolved2);
        Assert.AreNotEqual(resolved1, resolved2);
    }

    [Test]
    public void ResolveDependenciesFromPrefabInAllComponents() {
        var container = new UnityDIContainer();

        container.RegisterComponent<NoDependencyComponent>().FromPrefab(Resources.Load<NoDependencyComponent>("none")).AsSingleton();
        container.RegisterComponent<ConcreteDependencyComponent>().FromPrefab(Resources.Load<ConcreteDependencyComponent>("concrete")).AsSingleton();
        container.RegisterComponent<AbstractDependencyComponent>().FromPrefab(Resources.Load<AbstractDependencyComponent>("abstract")).AsSingleton();

        var none = container.Resolve<NoDependencyComponent>();
        var concrete = container.Resolve<ConcreteDependencyComponent>();
        var _abstract = container.Resolve<AbstractDependencyComponent>();

        Assert.IsNotNull(none);
        Assert.IsNotNull(concrete);
        Assert.IsNotNull(_abstract);
        Assert.AreEqual(none, concrete.Concrete);
        Assert.AreEqual(none, _abstract.Abstract);

        container = new UnityDIContainer();

        container.RegisterComponent<NoDependencyComponent>().FromPrefab(Resources.Load<NoDependencyComponent>("none")).AsTransient();
        container.RegisterComponent<ConcreteDependencyComponent>().FromPrefab(Resources.Load<ConcreteDependencyComponent>("concrete")).AsSingleton();
        container.RegisterComponent<AbstractDependencyComponent>().FromPrefab(Resources.Load<AbstractDependencyComponent>("abstract")).AsSingleton();

        none = container.Resolve<NoDependencyComponent>();
        concrete = container.Resolve<ConcreteDependencyComponent>();
        _abstract = container.Resolve<AbstractDependencyComponent>();

        Assert.IsNotNull(none);
        Assert.IsNotNull(concrete);
        Assert.IsNotNull(_abstract);
        Assert.AreNotEqual(none, concrete.Concrete);
        Assert.AreNotEqual(none, _abstract.Abstract);
        Assert.AreNotEqual(concrete.Concrete, _abstract.Abstract);

        container = new UnityDIContainer();

        container.RegisterComponent<NoDependencyComponent>().FromPrefab(Resources.Load<NoDependencyComponent>("none")).AsSingleton();
        container.RegisterComponent<ConcreteDependencyComponent>().FromPrefab(Resources.Load<ConcreteDependencyComponent>("concrete")).AsTransient();
        container.RegisterComponent<AbstractDependencyComponent>().FromPrefab(Resources.Load<AbstractDependencyComponent>("abstract")).AsTransient();

        none = container.Resolve<NoDependencyComponent>();
        concrete = container.Resolve<ConcreteDependencyComponent>();
        var concrete2 = container.Resolve<ConcreteDependencyComponent>();
        _abstract = container.Resolve<AbstractDependencyComponent>();
        var _abstract2 = container.Resolve<AbstractDependencyComponent>();

        Assert.IsNotNull(none);
        Assert.IsNotNull(concrete);
        Assert.IsNotNull(_abstract);
        Assert.AreEqual(none, concrete.Concrete);
        Assert.AreEqual(none, _abstract.Abstract);
        Assert.AreEqual(concrete2.Concrete, concrete.Concrete);
        Assert.AreEqual(_abstract2.Abstract, _abstract.Abstract);
        Assert.AreNotEqual(concrete2, concrete);
        Assert.AreNotEqual(_abstract2, _abstract);

        container = new UnityDIContainer();

        container.RegisterComponent<NoDependencyComponent>().FromPrefab(Resources.Load<NoDependencyComponent>("none")).AsTransient();
        container.RegisterComponent<ConcreteDependencyComponent>().FromPrefab(Resources.Load<ConcreteDependencyComponent>("concrete")).AsTransient();
        container.RegisterComponent<AbstractDependencyComponent>().FromPrefab(Resources.Load<AbstractDependencyComponent>("abstract")).AsTransient();

        none = container.Resolve<NoDependencyComponent>();
        concrete = container.Resolve<ConcreteDependencyComponent>();
        concrete2 = container.Resolve<ConcreteDependencyComponent>();
        _abstract = container.Resolve<AbstractDependencyComponent>();
        _abstract2 = container.Resolve<AbstractDependencyComponent>();

        Assert.IsNotNull(none);
        Assert.IsNotNull(concrete);
        Assert.IsNotNull(_abstract);
        Assert.AreNotEqual(none, concrete.Concrete);
        Assert.AreNotEqual(none, _abstract.Abstract);
        Assert.AreNotEqual(concrete.Concrete, _abstract.Abstract);
        Assert.AreNotEqual(concrete2.Concrete, concrete.Concrete);
        Assert.AreNotEqual(_abstract2.Abstract, _abstract.Abstract);
        Assert.AreNotEqual(concrete2, concrete);
        Assert.AreNotEqual(_abstract2, _abstract);
    }
}
