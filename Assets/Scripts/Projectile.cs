using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

    public LayerMask collisionMask;
    public float speed = 10;
    public float damage = 1;

    private float moveDistance;
    private Ray ray;
    private RaycastHit hit; 
	
	public void SetSpeed(float newSpeed) {
        speed = newSpeed;
    }

	void Update () {
        moveDistance = speed * Time.deltaTime;
        CheckCollisions(moveDistance);
        transform.Translate(Vector3.forward * Time.deltaTime * speed); 
	}

    void CheckCollisions(float _moveDistance) {
        ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out hit, _moveDistance, collisionMask,
            QueryTriggerInteraction.Collide)) { OnHitObject(hit); }
    }

    void OnHitObject(RaycastHit _hit) {
        IDamageable damageableObject = hit.collider.GetComponent<IDamageable>();
        if (damageableObject != null) damageableObject.TakeHit(damage, _hit);
        GameObject.Destroy(gameObject);
    }
}
