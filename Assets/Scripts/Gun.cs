using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {

    public enum FireMode {Auto, Burst, Single};
    public FireMode fireMode;

    public Transform[] projectileSpawn;
    public Projectile projectile;
    public float msBetweenShots = 100;
    public float muzzleVelocity = 35;
    public int burstCount;

    public GameObject shell;
    public Transform shellEjector;
    MuzzleFlash muzzleFlash;

    bool triggerReleasedSinceLastShot;
    int shotsRemainingInBurst;

    void Start() {
        muzzleFlash = GetComponent<MuzzleFlash>();
        shotsRemainingInBurst = burstCount;
    }

    private float nextShotTime;

    void Shoot() {

        if (Time.time > nextShotTime) {
            if (fireMode == FireMode.Burst) {
                if (shotsRemainingInBurst == 0) return;
                shotsRemainingInBurst--;
            } else if (fireMode == FireMode.Single) {
            if (!triggerReleasedSinceLastShot) return;
            }


            for (int i = 0; i < projectileSpawn.Length; i++) {
                nextShotTime = Time.time + msBetweenShots / 1000;
                Projectile newProjectile = (Projectile)Instantiate(projectile, projectileSpawn[i].position, projectileSpawn[i].rotation);
                newProjectile.SetSpeed(muzzleVelocity);
            }

            Instantiate(shell, shellEjector.position, shellEjector.rotation);
            muzzleFlash.Activate(); 
        }
    }

    public void OnTriggerHold() {
        Shoot();
        triggerReleasedSinceLastShot = false;
    }

    public void OnTriggerRelease() {
        triggerReleasedSinceLastShot = true;
        shotsRemainingInBurst = burstCount;
    }
}
