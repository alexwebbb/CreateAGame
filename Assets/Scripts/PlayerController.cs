using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
public class PlayerController : MonoBehaviour {

    Vector3 velocity;
    private Rigidbody myRigidbody;
    private Vector3 heightCorrectedPoint;
	
	void Start () {
        myRigidbody = GetComponent<Rigidbody>();
	}
	
	public void Move(Vector3 _velocity) {
        velocity = _velocity;
    }

    public void LookAt(Vector3 lookPoint) {
        heightCorrectedPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
        transform.LookAt(heightCorrectedPoint);
    }

    void FixedUpdate() {
        myRigidbody.MovePosition(myRigidbody.position + velocity * Time.fixedDeltaTime);
    }

	void Update () {
	    
	}
}
