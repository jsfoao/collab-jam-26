using System;
using System.Collections.Generic;
using UnityEngine;

public interface IDestructible
{
    public void OnDestruct();
}

[RequireComponent(typeof(Rigidbody))]
public class Destructible : MonoBehaviour, IDestructible
{
    [SerializeField] public float health = 100f;
    [SerializeField] public float maxDamagePerHit = 10f;
    [SerializeField] public float minDamageSpeed = 2f;
    [SerializeField] public bool scaleDamageWithSpeed = true;
    [SerializeField] public bool destroyOnDestruct = true;

    [NonSerialized] public List<IDestructible> contexts = new List<IDestructible>();

    void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (!rb)
        {
            return;
        }
        float speed = collision.relativeVelocity.magnitude;
        if (speed < minDamageSpeed)
        {
            return;
        }
        float damage = maxDamagePerHit;
        if (scaleDamageWithSpeed)
        {
            damage *= speed;
        } 
        TakeDamage(damage);
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0f)
        {
            return;
        }

        health -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage, health is now {health}");
        if (health <= 0f)
        {
            Destruct();
        }
    }

    public void Destruct()
    {
        health = 0f;
        OnDestruct();
    }

    public void OnDestruct()
    {
        foreach (IDestructible context in contexts)
        {
            if (context == null)
            {
                continue;
            }
            context.OnDestruct();
        }

        if (destroyOnDestruct)
        {
            Destroy(gameObject);
        }
    }
}