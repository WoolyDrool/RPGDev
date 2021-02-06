using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGSystem.Core.Items
{
    [CreateAssetMenu(menuName = "Items/New Weapon")]
    public class FirearmWeaponData : BaseWeapon
    {
        [Space(5)]
        [Header("Weapon Variables")]
        //Type
        public FiringType firingType;
        public enum FiringType { SEMIAUTO, BURST, FULLAUTO }
        public int fRequirement;
        [Space(5)]
        [Header("Ammo")]
        //Ammo Type
        public AmmoType ammoType;
        public GameObject bullet;
        public enum AmmoType { PISTOL, RIFLE, SHOTGUN, HEAVY, ENERGY }
        //Ammo
        public int ammoPerShot;
        public int clipSize;
        public float reloadSpeed;
        bool canReload;
        bool isReloading;
        [Space(5)]
        [Header("Damage")]
        //Damage
        public float firingSpeed;
        public int projectileSpeed;
        public int burstSize;
        public float burstInterval;
        [Space(5)]
        [Header("Accuracy")]
        public float accuracyPenalty;
        public float projectileRange;
        public float kickBackPowerHip;
        public float kickBackPowerAim;
        public float recoilSpeed;
        public float returnSpeed;
        [Header("Sound/VFX")]
        //Sound
        public AudioClip firingSFX;
        //Muzzle flash prefab
        public GameObject muzzleFlash;

    }
}
