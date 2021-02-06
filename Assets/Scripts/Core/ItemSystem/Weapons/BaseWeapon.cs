using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGSystem.Core.Items
{
    public class BaseWeapon : ItemData
    {
        /*public ObjectType weaponType;
        public enum ObjectType{MELEE, GUN}*/
        public int baseDamage;
        public FirearmWeaponData firearmData;
        public MeleeWeaponData meleeWeaponData;
        public bool isEquipped = false;
    }
}
