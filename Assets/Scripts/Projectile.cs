using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

    public LayerMask collisionMask;
    public float damage = 1;
    public float lifetime = 3;

    private float speed;
    private float moveDistance;
    private Ray ray;
    private RaycastHit hit; 
	
	public void SetSpeed(float newSpeed) {
        speed = newSpeed;
    }

    void Start () {
        Destroy(gameObject, lifetime);

        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, .1f, collisionMask);
        if (initialCollisions.Length > 0) OnHitObject(initialCollisions[0], transform.position);
    }

	void Update () {
        moveDistance = speed * Time.deltaTime;
        CheckCollisions(moveDistance);
        transform.Translate(Vector3.forward * Time.deltaTime * speed); 
	}

    void CheckCollisions(float _moveDistance) {
        ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out hit, _moveDistance, collisionMask,
            QueryTriggerInteraction.Collide)) { OnHitObject(hit.collider, hit.point); }
    }

    void OnHitObject(Collider c, Vector3 hitPoint) {
        IDamageable damageableObject = c.GetComponent<IDamageable>();
        if (damageableObject != null) damageableObject.TakeHit(damage, hitPoint, transform.forward);
        GameObject.Destroy(gameObject);
    }
}
