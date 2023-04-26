using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private GameObject playerRef;
    [SerializeField] private Weapon[] weapons;
    [SerializeField] private Camera fpsCam;

    private int currentWeaponIndex;

    // Start is called before the first frame update
    void Start()
    {
        currentWeaponIndex = 0;
        weapons[currentWeaponIndex].gameObject.SetActive(true);

        foreach(Weapon weapon in weapons)
        {
            weapon.InitializeRefs(playerRef.GetComponent<Rigidbody>(), fpsCam);
        }
    }

    // Update is called once per frame
    void Update()
    {
        weapons[currentWeaponIndex].PlayerInputWeaponAction();
    }
    
    //handles inputs for swapping weapons
    public void SwapWeapons(int weaponIndex)
    {
        //if (PlayerShoot.reloading == false) {
        if (currentWeaponIndex == weaponIndex || weaponIndex > weapons.Length - 1)
            return;

        weapons[currentWeaponIndex].gameObject.SetActive(false);
        weapons[weaponIndex].gameObject.SetActive(true);
        currentWeaponIndex = weaponIndex;
        //}
    }

    public Weapon GetCurrentSelectedWeapon()
    {
        return weapons[currentWeaponIndex];
    }
}
