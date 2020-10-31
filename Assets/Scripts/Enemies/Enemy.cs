using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityCore.Data;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, IDamageable, ISavable
{

    [SerializeField] protected float moveSpeed = 1f;
    [SerializeField] protected int health = 100;
    [SerializeField] protected int damage = 20;

    protected abstract void Move();

    protected bool IsFacingRigth()
    {
        return transform.localScale.x > 0;
    }

    public int GetDamage()
    {
        return damage;
    }

    public abstract void TakeDamage(int damage);
    public abstract void TriggerDeath();
    
    public abstract object CaptureState();
    public abstract void RestoreState(object state);
}
