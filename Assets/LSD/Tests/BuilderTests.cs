using NUnit.Framework;
using UnityEngine.TestTools;
using LSD;
using LSD.Builder;

public class BuilderTests
{
    [Test]
    public void CompilesAndWorks() {
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
    public void CompilesAndWorksDerived() {
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
