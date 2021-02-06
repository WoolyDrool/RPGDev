using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPGSystem.Core.Items;

namespace RPGSystem.Core.Inventory
{
    public class WeaponManager : MonoBehaviour
    {
        //Gun In Hands
        public BaseWeapon currentWeapon;
        public GameObject weaponInstance;

        //Weapon to add from Inventory
        BaseWeapon weaponToAdd;
        GameObject weaponToRemove;
        public bool hasWeaponEquipped;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.H) && hasWeaponEquipped)
            {
                RemoveWeapon();
            }
        }

        public void ReceiveWeapon(BaseWeapon weaponData)
        {
            weaponToAdd = weaponData;
            weaponToRemove = weaponInstance;
            if (hasWeaponEquipped)
            {
                weaponInstance.SendMessage("Unequip");
                SwitchWeapon();
            }
            else
            {
                AddWeapon();
            }

        }

        public void AddWeapon()
        {
            currentWeapon = weaponToAdd;
            weaponInstance = Instantiate(weaponToAdd.sceneInstance, transform.position, transform.rotation);
            weaponInstance.transform.parent = this.transform;
            weaponInstance.SendMessage("Equip");
            hasWeaponEquipped = true;
        }

        public void RemoveWeapon()
        {
            weaponInstance.SendMessage("Unequip");
            currentWeapon = null;
            Destroy(weaponInstance);
            weaponInstance = null;
            hasWeaponEquipped = false;
        }

        public void SwitchWeapon()
        {
            currentWeapon.isEquipped = false;
            RemoveWeapon();

            AddWeapon();
            currentWeapon.isEquipped = true;
        }
    }
}
