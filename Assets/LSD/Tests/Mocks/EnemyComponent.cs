using System;
using LSD;
using LSD.Unity;
using UnityEngine;

public class EnemyComponent : MonoBehaviour, IEnemyComponent, ICloneable
{
    [Dependency]
    private EnemyData data;
    public EnemyData Data => data;

    [Dependency]
    private Guid id;
    public Guid Id => id;

    internal class Builder : UnityBuilder<EnemyComponent> { }

    public object Clone()
    {
        return Instantiate(this);
    }
}

public interface IEnemyComponent
{
    EnemyData Data { get; }
}