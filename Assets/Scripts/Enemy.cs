using UnityEngine;
using System.Collections;

[RequireComponent (typeof (NavMeshAgent))]
public class Enemy : MonoBehaviour {

    public float refreshRate = .25f;

    private NavMeshAgent pathfinder;
    private Transform target;
    private Vector3 targetPosition;


    void Start () {
        pathfinder = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player").transform;

        StartCoroutine(UpdatePath());
	
	}
	
	void Update () {
        
	}

    IEnumerator UpdatePath() {
        while (target != null) {
            targetPosition = new Vector3(target.position.x, 0, target.position.z);
            pathfinder.SetDestination(targetPosition);
            yield return new WaitForSeconds(refreshRate); 
        }
    }
}
