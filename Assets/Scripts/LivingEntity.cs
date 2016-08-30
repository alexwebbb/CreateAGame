using UnityEngine;
using System.Collections;

public class LivingEntity : MonoBehaviour, IDamageable {

    public float startingHealth = 3;

    protected float health;
    protected bool dead;

    public event System.Action OnDeath;

    protected virtual void Start() {
        health = startingHealth;
    }

    public void TakeHit(float damage, RaycastHit hit) {
        // do some stuff with the hit variable
        TakeDamage(damage);
    }

    public void TakeDamage(float damage) {
        health -= damage;
        // print("damage");
        if (health <= 0 && !dead) {
            Die();
        }
    }

    protected void Die() {
        dead = true;
        if (OnDeath != null) OnDeath();
        GameObject.Destroy(gameObject);
    }
	
}
