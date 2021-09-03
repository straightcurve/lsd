using System;
using NUnit.Framework;
using UnityEngine.TestTools;
using Syringe;

public class DIContainerTests
{
    internal class RandomProvider : IProvider {
        private readonly Guid value = Guid.NewGuid();
        public Guid Value => value;
    }

    internal interface IProvider {
        Guid Value { get; }
    }

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
    public void ConcreteSingletonNoDependencyWithInitialValue() {
        var container = new DIContainer();

        container.RegisterSingleton(new RandomProvider());

        var rnd1 = container.Resolve<RandomProvider>();
        var rnd2 = container.Resolve<RandomProvider>();

        Assert.AreEqual(rnd1.Value, rnd2.Value);
    }

    [Test]
    public void ConcreteSingletonNoDependencyNonLazy() {
        var container = new DIContainer();

        container.RegisterSingleton<RandomProvider>();

        var rnd1 = container.Resolve<RandomProvider>();
        var rnd2 = container.Resolve<RandomProvider>();

        Assert.AreEqual(rnd1.Value, rnd2.Value);
    }
    
    [Test]
    public void AbstractSingletonNoDependency() {
        var container = new DIContainer();

        container.RegisterSingleton<IProvider, RandomProvider>();

        var rnd1 = container.Resolve<IProvider>();
        var rnd2 = container.Resolve<IProvider>();

        Assert.AreEqual(rnd1.Value, rnd2.Value);
    }

    [Test]
    public void AbstractSingletonNoDependencyWithInitialValue() {
        var container = new DIContainer();

        container.RegisterSingleton<IProvider, RandomProvider>(new RandomProvider());

        var rnd1 = container.Resolve<IProvider>();
        var rnd2 = container.Resolve<IProvider>();

        Assert.AreEqual(rnd1.Value, rnd2.Value);
    }

    [Test]
    public void ConcreteTransientNoDependency() {
        var container = new DIContainer();

        container.RegisterTransient<RandomProvider>();

        var rnd1 = container.Resolve<RandomProvider>();
        var rnd2 = container.Resolve<RandomProvider>();

        Assert.AreNotEqual(rnd1.Value, rnd2.Value);
    }

    [Test]
    public void AbstractTransientNoDependency() {
        var container = new DIContainer();

        container.RegisterTransient<IProvider, RandomProvider>();

        var rnd1 = container.Resolve<IProvider>();
        var rnd2 = container.Resolve<IProvider>();

        Assert.AreNotEqual(rnd1.Value, rnd2.Value);
    }

    [Test]
    public void SingletonConcreteSingletonDependency() {
        var container = new DIContainer();

        container.RegisterSingleton<RandomProvider>();
        container.RegisterSingleton<ConcreteDependencyService>();

        var rnd1 = container.Resolve<RandomProvider>();
        var rnd2 = container.Resolve<ConcreteDependencyService>();

        Assert.AreEqual(rnd1.Value, rnd2.ConcreteValue);
    }

    [Test]
    public void SingletonAbstractSingletonDependency() {
        var container = new DIContainer();

        container.RegisterSingleton<IProvider, RandomProvider>();
        container.RegisterSingleton<AbstractDependencyService>();

        var rnd1 = container.Resolve<IProvider>();
        var rnd2 = container.Resolve<AbstractDependencyService>();

        Assert.IsNotNull(rnd1);
        Assert.IsNotNull(rnd2);
        Assert.AreEqual(rnd1.Value, rnd2.AbstractValue);
    }

    [Test]
    public void SingletonConcreteTransientDependency() {
        var container = new DIContainer();

        container.RegisterTransient<RandomProvider>();
        container.RegisterSingleton<ConcreteDependencyService>();

        var rnd1 = container.Resolve<RandomProvider>();
        var rnd2 = container.Resolve<ConcreteDependencyService>();

        Assert.IsNotNull(rnd1);
        Assert.IsNotNull(rnd2);
        Assert.AreNotEqual(rnd1.Value, rnd2.ConcreteValue);
    }

    [Test]
    public void SingletonAbstractTransientDependency() {
        var container = new DIContainer();

        container.RegisterTransient<IProvider, RandomProvider>();
        container.RegisterSingleton<AbstractDependencyService>();

        var rnd1 = container.Resolve<IProvider>();
        var rnd2 = container.Resolve<AbstractDependencyService>();

        Assert.AreNotEqual(rnd1.Value, rnd2.AbstractValue);
    }
    
    [Test]
    public void TransientConcreteSingletonDependency() {
        var container = new DIContainer();

        container.RegisterSingleton<RandomProvider>();
        container.RegisterTransient<ConcreteDependencyService>();

        var rnd1 = container.Resolve<ConcreteDependencyService>();
        var rnd2 = container.Resolve<ConcreteDependencyService>();

        Assert.AreEqual(rnd1.ConcreteValue, rnd2.ConcreteValue);
        Assert.AreNotEqual(rnd1, rnd2);
    }
    
    [Test]
    public void TransientAbstractSingletonDependency() {
        var container = new DIContainer();

        container.RegisterSingleton<IProvider, RandomProvider>();
        container.RegisterTransient<AbstractDependencyService>();

        var rnd1 = container.Resolve<AbstractDependencyService>();
        var rnd2 = container.Resolve<AbstractDependencyService>();

        Assert.AreEqual(rnd1.AbstractValue, rnd2.AbstractValue);
        Assert.AreNotEqual(rnd1, rnd2);
    }

    [Test]
    public void TransientConcreteTransientDependency() {
        var container = new DIContainer();

        container.RegisterTransient<RandomProvider>();
        container.RegisterTransient<ConcreteDependencyService>();

        var rnd1 = container.Resolve<ConcreteDependencyService>();
        var rnd2 = container.Resolve<ConcreteDependencyService>();

        Assert.AreNotEqual(rnd1.ConcreteValue, rnd2.ConcreteValue);
        Assert.AreNotEqual(rnd1, rnd2);
    }

    [Test]
    public void TransientAbstractTransientDependency() {
        var container = new DIContainer();

        container.RegisterTransient<IProvider, RandomProvider>();
        container.RegisterTransient<AbstractDependencyService>();

        var rnd1 = container.Resolve<AbstractDependencyService>();
        var rnd2 = container.Resolve<AbstractDependencyService>();

        Assert.AreNotEqual(rnd1.AbstractValue, rnd2.AbstractValue);
        Assert.AreNotEqual(rnd1, rnd2);
    }

    [Test]
    public void ResolveConcreteSingletonFromParentContainerWithInitialValue() {
        var parent = new DIContainer();
        var container = new DIContainer(parent);

        parent.RegisterSingleton(new RandomProvider());

        var rnd1 = parent.Resolve<RandomProvider>();
        var rnd2 = container.Resolve<RandomProvider>();

        Assert.AreEqual(rnd1.Value, rnd2.Value);
    }

    [Test]
    public void ResolveConcreteSingletonFromParentContainerNonLazy() {
        var parent = new DIContainer();
        var container = new DIContainer(parent);

        parent.RegisterSingleton<RandomProvider>();

        var rnd1 = parent.Resolve<RandomProvider>();
        var rnd2 = container.Resolve<RandomProvider>();

        Assert.AreEqual(rnd1.Value, rnd2.Value);
    }

    [Test]
    public void ResolveAbstractSingletonFromParentContainerNonLazy() {
        var parent = new DIContainer();
        var container = new DIContainer(parent);

        parent.RegisterSingleton<IProvider, RandomProvider>();

        var rnd1 = parent.Resolve<IProvider>();
        var rnd2 = container.Resolve<IProvider>();

        Assert.AreEqual(rnd1.Value, rnd2.Value);
    }

    [Test]
    public void ResolveAbstractSingletonFromParentContainerWithInitialValue() {
        var parent = new DIContainer();
        var container = new DIContainer(parent);

        parent.RegisterSingleton<IProvider, RandomProvider>(new RandomProvider());

        var rnd1 = parent.Resolve<IProvider>();
        var rnd2 = container.Resolve<IProvider>();

        Assert.AreEqual(rnd1.Value, rnd2.Value);
    }

    [Test]
    public void ResolveConcreteTransientFromParentContainerNonLazy() {
        var parent = new DIContainer();
        var container = new DIContainer(parent);

        parent.RegisterTransient<RandomProvider>();

        var rnd1 = parent.Resolve<RandomProvider>();
        var rnd2 = container.Resolve<RandomProvider>();

        Assert.AreNotEqual(rnd1.Value, rnd2.Value);
    }

    [Test]
    public void ResolveAbstractTransientFromParentContainerNonLazy() {
        var parent = new DIContainer();
        var container = new DIContainer(parent);

        parent.RegisterTransient<IProvider, RandomProvider>();

        var rnd1 = parent.Resolve<IProvider>();
        var rnd2 = container.Resolve<IProvider>();

        Assert.AreNotEqual(rnd1.Value, rnd2.Value);
    }
}
