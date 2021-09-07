using NUnit.Framework;
using UnityEngine.TestTools;
using LSD;
using System;

public class SyringeTests
{
    [Test]
    public void InjectsDependenciesInAllFields()
    {
        var container = new DIContainer();
        container.Register<string>().FromInstance("221B Baker Street");
        container.Register<Address>().FromNew().AsTransient();

        container.Register<Builder<User>>().FromNew().AsSingleton();
        var builder = container.Resolve<Builder<User>>();

        var user = builder.New()
            .Override<int, User>(420)
            .Override<string, User>("Sherlock Holmes").Build();

        Assert.IsNotNull(user);
        Assert.IsNotNull(user.Address);
        Assert.AreEqual("Sherlock Holmes", user.Name);
        Assert.AreEqual("221B Baker Street", user.Address.Street);
    }
}
