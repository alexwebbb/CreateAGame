using UnityEngine;
using System.Collections;

[RequireComponent (typeof (PlayerController))]
public class Player : MonoBehaviour {

    public float moveSpeed = 5;

    private Vector3 moveInput;
    private Vector3 moveVelocity;
    private Camera viewCamera;
    private PlayerController myController;

    private Ray ray;
    private Plane groundPlane;
    private float rayDistance;

	void Start () {
        myController = GetComponent<PlayerController>();
        viewCamera = Camera.main;
	}
	
	
	void Update () {
        moveInput = new Vector3( Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        moveVelocity = moveInput.normalized * moveSpeed;
        myController.Move(moveVelocity);

        ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out rayDistance)) {
            Vector3 point = ray.GetPoint(rayDistance);
            // Debug.DrawLine(ray.origin, point, Color.red);
            myController.LookAt(point);
        }
	}
}
