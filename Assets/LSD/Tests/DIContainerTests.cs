using System;
using NUnit.Framework;
using UnityEngine.TestTools;
using LSD;

public class DIContainerTests
{
    internal class ConcreteDependencyService : IConcrete {

        [Dependency]
        private RandomProvider concrete;

        public Guid ConcreteValue => concrete.Value;
    }

    internal class AbstractDependencyService : IAbstract {

        [Dependency]
        private IProvider _abstract;

        public Guid AbstractValue => _abstract.Value;
    }

    internal class BothDependencyService : IBoth {

        [Dependency]
        private RandomProvider concrete;

        [Dependency]
        private IProvider _abstract;

        public Guid AbstractValue => _abstract.Value;
        public Guid ConcreteValue => concrete.Value;
    }

    internal interface IConcrete {
        Guid ConcreteValue { get; }
    }

    internal interface IAbstract {
        Guid AbstractValue { get; }
    }

    internal interface IBoth : IConcrete, IAbstract { }

    [Test]
    public void ResolveConcreteFromNewSingleton() {
        var container = new DIContainer();

        container.Register<RandomProvider>().FromNew().AsSingleton();

        var rnd1 = container.Resolve<RandomProvider>();
        var rnd2 = container.Resolve<RandomProvider>();

        Assert.AreEqual(rnd1.Value, rnd2.Value);
    }
    
    [Test]
    public void ResolveAbstractFromNewSingleton() {
        var container = new DIContainer();

        container.Register<IProvider, RandomProvider>().FromNew().AsSingleton();

        var rnd1 = container.Resolve<IProvider>();
        var rnd2 = container.Resolve<IProvider>();

        Assert.AreEqual(rnd1.Value, rnd2.Value);
    }

    [Test]
    public void ResolveConcreteFromInstance() {
        var container = new DIContainer();

        container.Register<RandomProvider>().FromInstance(new RandomProvider());

        var rnd1 = container.Resolve<RandomProvider>();
        var rnd2 = container.Resolve<RandomProvider>();

        Assert.AreEqual(rnd1.Value, rnd2.Value);
        Assert.AreEqual(rnd1, rnd2);
    }

    [Test]
    public void ResolveAbstractFromInstance() {
        var container = new DIContainer();

        container.Register<IProvider, RandomProvider>().FromInstance(new RandomProvider());

        var rnd1 = container.Resolve<IProvider>();
        var rnd2 = container.Resolve<IProvider>();

        Assert.AreEqual(rnd1.Value, rnd2.Value);
    }

    [Test]
    public void ResolveConcreteFromNewTransient() {
        var container = new DIContainer();

        container.Register<RandomProvider>().FromNew().AsTransient();

        var rnd1 = container.Resolve<RandomProvider>();
        var rnd2 = container.Resolve<RandomProvider>();

        Assert.AreNotEqual(rnd1.Value, rnd2.Value);
    }

    [Test]
    public void ResolveAbstractFromNewTransient() {
        var container = new DIContainer();

        container.Register<IProvider, RandomProvider>().FromNew().AsTransient();

        var rnd1 = container.Resolve<IProvider>();
        var rnd2 = container.Resolve<IProvider>();

        Assert.AreNotEqual(rnd1.Value, rnd2.Value);
    }

    [Test]
    public void ResolveConcreteFromNewSingletonForFromNewSingleton() {
        var container = new DIContainer();

        container.Register<RandomProvider>().FromNew().AsSingleton();
        container.Register<ConcreteDependencyService>().FromNew().AsSingleton();

        var rnd1 = container.Resolve<RandomProvider>();
        var rnd2 = container.Resolve<ConcreteDependencyService>();

        Assert.AreEqual(rnd1.Value, rnd2.ConcreteValue);
    }

    [Test]
    public void ResolveAbstractFromNewSingletonForFromNewSingleton() {
        var container = new DIContainer();

        container.Register<IProvider, RandomProvider>().FromNew().AsSingleton();
        container.Register<AbstractDependencyService>().FromNew().AsSingleton();

        var rnd1 = container.Resolve<IProvider>();
        var rnd2 = container.Resolve<AbstractDependencyService>();

        Assert.IsNotNull(rnd1);
        Assert.IsNotNull(rnd2);
        Assert.AreEqual(rnd1.Value, rnd2.AbstractValue);
    }

    [Test]
    public void ResolveConcreteFromNewTransientForFromNewSingleton() {
        var container = new DIContainer();

        container.Register<RandomProvider>().FromNew().AsTransient();
        container.Register<ConcreteDependencyService>().FromNew().AsSingleton();

        var rnd1 = container.Resolve<RandomProvider>();
        var rnd2 = container.Resolve<ConcreteDependencyService>();

        Assert.IsNotNull(rnd1);
        Assert.IsNotNull(rnd2);
        Assert.AreNotEqual(rnd1.Value, rnd2.ConcreteValue);
    }

    [Test]
    public void ResolveAbstractFromNewTransientForFromNewSingleton() {
        var container = new DIContainer();

        container.Register<IProvider, RandomProvider>().FromNew().AsTransient();
        container.Register<AbstractDependencyService>().FromNew().AsSingleton();

        var rnd1 = container.Resolve<IProvider>();
        var rnd2 = container.Resolve<AbstractDependencyService>();

        Assert.AreNotEqual(rnd1.Value, rnd2.AbstractValue);
    }
    
    [Test]
    public void ResolveConcreteFromNewSingletonForFromNewTransient() {
        var container = new DIContainer();

        container.Register<RandomProvider>().FromNew().AsSingleton();
        container.Register<ConcreteDependencyService>().FromNew().AsTransient();

        var rnd1 = container.Resolve<ConcreteDependencyService>();
        var rnd2 = container.Resolve<ConcreteDependencyService>();

        Assert.AreEqual(rnd1.ConcreteValue, rnd2.ConcreteValue);
        Assert.AreNotEqual(rnd1, rnd2);
    }
    
    [Test]
    public void ResolveAbstractFromNewSingletonForFromNewTransient() {
        var container = new DIContainer();

        container.Register<IProvider, RandomProvider>().FromNew().AsSingleton();
        container.Register<AbstractDependencyService>().FromNew().AsTransient();

        var rnd1 = container.Resolve<AbstractDependencyService>();
        var rnd2 = container.Resolve<AbstractDependencyService>();

        Assert.AreEqual(rnd1.AbstractValue, rnd2.AbstractValue);
        Assert.AreNotEqual(rnd1, rnd2);
    }

    [Test]
    public void ResolveConcreteFromNewTransientForFromNewTransient() {
        var container = new DIContainer();

        container.Register<RandomProvider>().FromNew().AsTransient();
        container.Register<ConcreteDependencyService>().FromNew().AsTransient();

        var rnd1 = container.Resolve<ConcreteDependencyService>();
        var rnd2 = container.Resolve<ConcreteDependencyService>();

        Assert.AreNotEqual(rnd1.ConcreteValue, rnd2.ConcreteValue);
        Assert.AreNotEqual(rnd1, rnd2);
    }

    [Test]
    public void ResolveAbstractFromNewTransientForFromNewTransient() {
        var container = new DIContainer();

        container.Register<IProvider, RandomProvider>().FromNew().AsTransient();
        container.Register<AbstractDependencyService>().FromNew().AsTransient();

        var rnd1 = container.Resolve<AbstractDependencyService>();
        var rnd2 = container.Resolve<AbstractDependencyService>();

        Assert.AreNotEqual(rnd1.AbstractValue, rnd2.AbstractValue);
        Assert.AreNotEqual(rnd1, rnd2);
    }

    [Test]
    public void ResolveConcreteFromInstanceFromParentContainer() {
        var parent = new DIContainer();
        var container = new DIContainer(parent);

        parent.Register<RandomProvider>().FromInstance(new RandomProvider());

        var rnd1 = parent.Resolve<RandomProvider>();
        var rnd2 = container.Resolve<RandomProvider>();

        Assert.AreEqual(rnd1.Value, rnd2.Value);
    }

    [Test]
    public void ResolveConcreteFromNewSingletonFromParentContainer() {
        var parent = new DIContainer();
        var container = new DIContainer(parent);

        parent.Register<RandomProvider>().FromNew().AsSingleton();

        var rnd1 = parent.Resolve<RandomProvider>();
        var rnd2 = container.Resolve<RandomProvider>();

        Assert.AreEqual(rnd1.Value, rnd2.Value);
    }

    [Test]
    public void ResolveAbstractFromNewSingletonFromParentContainer() {
        var parent = new DIContainer();
        var container = new DIContainer(parent);

        parent.Register<IProvider, RandomProvider>().FromNew().AsSingleton();

        var rnd1 = parent.Resolve<IProvider>();
        var rnd2 = container.Resolve<IProvider>();

        Assert.AreEqual(rnd1.Value, rnd2.Value);
    }

    [Test]
    public void ResolveAbstractFromInstanceFromParentContainer() {
        var parent = new DIContainer();
        var container = new DIContainer(parent);

        parent.Register<IProvider, RandomProvider>().FromInstance(new RandomProvider());

        var rnd1 = parent.Resolve<IProvider>();
        var rnd2 = container.Resolve<IProvider>();

        Assert.AreEqual(rnd1.Value, rnd2.Value);
    }

    [Test]
    public void ResolveConcreteFromNewTransientFromParentContainer() {
        var parent = new DIContainer();
        var container = new DIContainer(parent);

        parent.Register<RandomProvider>().FromNew().AsTransient();

        var rnd1 = parent.Resolve<RandomProvider>();
        var rnd2 = container.Resolve<RandomProvider>();

        Assert.AreNotEqual(rnd1.Value, rnd2.Value);
    }

    [Test]
    public void ResolveAbstractFromNewTransientFromParentContainer() {
        var parent = new DIContainer();
        var container = new DIContainer(parent);

        parent.Register<IProvider, RandomProvider>().FromNew().AsTransient();

        var rnd1 = parent.Resolve<IProvider>();
        var rnd2 = container.Resolve<IProvider>();

        Assert.AreNotEqual(rnd1.Value, rnd2.Value);
    }
}
