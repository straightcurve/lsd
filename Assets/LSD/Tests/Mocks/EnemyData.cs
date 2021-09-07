using System;
using LSD;
using LSD.Unity;
using UnityEngine;

[CreateAssetMenu(menuName = "New/Enemy", fileName = "NewEnemy")]
public class EnemyData : ScriptableObject
{
    private Guid id = Guid.NewGuid();
    public Guid Id => id;
}
