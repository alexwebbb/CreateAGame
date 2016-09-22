using UnityEngine;
using System.Collections;

[RequireComponent (typeof (PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity {

    public float moveSpeed = 5;

    public Crosshairs crosshairs;

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
        groundPlane = new Plane(Vector3.up, Vector3.up * gunController.GunHeight);

        if (groundPlane.Raycast(ray, out rayDistance)) {
            Vector3 point = ray.GetPoint(rayDistance);
            // Debug.DrawLine(ray.origin, point, Color.red);
            myController.LookAt(point);
            crosshairs.transform.position = point;
            crosshairs.DetectTargets(ray);

            if ((new Vector2(point.x, point.y) - new Vector2(transform.position.x, transform.position.y)).sqrMagnitude > 1) {
                gunController.Aim(point);
            }
        }

        // Weapon Input
        if (Input.GetMouseButton(0)) {
            gunController.OnTriggerHold();
        }
        if (Input.GetMouseButtonUp(0)) {
            gunController.OnTriggerRelease();
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            gunController.Reload();
        }
    }
}
