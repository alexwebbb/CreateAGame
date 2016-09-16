using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {

    public Transform muzzle;
    public Projectile projectile;
    public float msBetweenShots = 100;
    public float muzzleVelocity = 35;

    public GameObject shell;
    public Transform shellEjector;
    MuzzleFlash muzzleFlash;

    void Start() {
        muzzleFlash = GetComponent<MuzzleFlash>();
    }

    private float nextShotTime;

    public void Shoot() {

        if (Time.time > nextShotTime) {
            nextShotTime = Time.time + msBetweenShots / 1000;
            Projectile newProjectile = (Projectile)Instantiate(projectile, muzzle.position, muzzle.rotation);
            newProjectile.SetSpeed(muzzleVelocity);

            Instantiate(shell, shellEjector.position, shellEjector.rotation);
            muzzleFlash.Activate();
        }
    }
}
