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

    public virtual void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection) {
        // do some stuff with the hit variable
        TakeDamage(damage);
    }

    public virtual void TakeDamage(float damage) {
        health -= damage;
        // print("damage");
        if (health <= 0 && !dead) {
            Die();
        }
    }

    [ContextMenu("Self Destruct")]
    protected void Die() {
        dead = true;
        if (OnDeath != null) OnDeath();
        GameObject.Destroy(gameObject);
    }
	
}
