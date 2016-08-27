using UnityEngine;
using System.Collections;

[RequireComponent (typeof (NavMeshAgent))]
public class Enemy : LivingEntity {

    public enum State { Idle, Chasing, Attacking };

    public float refreshRate = .25f;
    public float atkDistThreshhold = .5f;
    public float timeBetweenAttacks = 1;
    public float attackSpeed = 3;

    Material skinMaterial;
    Color originalColor;
    
    private State currentState;

    private NavMeshAgent pathfinder;
    private Transform target;
    
    private float sqrAttackDistanceThreshhold;
    private float nextAttackTime;
    private float myColRadius;
    private float targetColRadius;

    protected override void Start () {
        base.Start();
        pathfinder = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player").transform;

        //set state
        currentState = State.Chasing;

        //initialize variables
        myColRadius = GetComponent<CapsuleCollider>().radius;
        targetColRadius = target.GetComponent<CapsuleCollider>().radius;
        sqrAttackDistanceThreshhold = Mathf.Pow(atkDistThreshhold + myColRadius + targetColRadius, 2);
        // color
        skinMaterial = GetComponent<Renderer>().material;
        originalColor = skinMaterial.color;

        StartCoroutine(UpdatePath());
	
	}
	
	void Update () {
        if (Time.time > nextAttackTime) {
            float sqrDistanceToTarget = (target.position - transform.position).sqrMagnitude;
            if (sqrDistanceToTarget < sqrAttackDistanceThreshhold) {
                nextAttackTime = Time.time + timeBetweenAttacks;
                StartCoroutine(Attack());
            } 
        }

	}

    IEnumerator Attack() {
        currentState = State.Attacking;
        pathfinder.enabled = false;

        Vector3 originalPostion = transform.position;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - dirToTarget * myColRadius;

        float percent = 0;

        skinMaterial.color = Color.red;

        while (percent <= 1) {
            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(originalPostion, attackPosition, interpolation);
            yield return null;
        }

        skinMaterial.color = originalColor;
        currentState = State.Chasing;
        pathfinder.enabled = true;
    }

    IEnumerator UpdatePath() {
        if(dead) yield break;
        while (target != null) {
            if (currentState == State.Chasing) {
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - dirToTarget * 
                    (myColRadius + targetColRadius + atkDistThreshhold/2);
                pathfinder.SetDestination(targetPosition); 
            }
            yield return new WaitForSeconds(refreshRate); 
        }
    }
}
