using UnityEngine;
using System.Collections;

public class GunController : MonoBehaviour {

    public Gun startingGun;
    public Transform weaponHold;

    private Gun equippedGun;

    void Start() {
        if (startingGun != null) EquipGun(startingGun);
    }

    public void EquipGun(Gun gunToEquip) {
        if (equippedGun != null) Destroy(equippedGun.gameObject);
        equippedGun = (Gun)Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation);
        equippedGun.transform.parent = weaponHold;
    }
	
    public void fireShot() {
        if (equippedGun != null) equippedGun.Shoot();
    }
}
