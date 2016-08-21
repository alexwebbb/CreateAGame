using UnityEngine;
using System.Collections;

[RequireComponent (typeof (PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity {

    public float moveSpeed = 5;

    private Vector3 moveInput;
    private Vector3 moveVelocity;
    private Camera viewCamera;
    private PlayerController myController;
    private GunController gunController;

    private Ray ray;
    private Plane groundPlane;
    private float rayDistance;

	protected override void Start () {
        base.Start();
        myController = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        viewCamera = Camera.main;
	}
	
	
	void Update () {
        // Movement Input
        moveInput = new Vector3( Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        moveVelocity = moveInput.normalized * moveSpeed;
        myController.Move(moveVelocity);

        // Look Input
        ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out rayDistance)) {
            Vector3 point = ray.GetPoint(rayDistance);
            // Debug.DrawLine(ray.origin, point, Color.red);
            myController.LookAt(point);
        }

        // Weapon Input
        if (Input.GetMouseButton(0)) {
            gunController.fireShot();
        }
	}
}
