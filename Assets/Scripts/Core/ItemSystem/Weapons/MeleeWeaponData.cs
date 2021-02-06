using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGSystem.Core.Items
{
    [CreateAssetMenu(menuName = "Items/New Melee Weapon")]
    public class MeleeWeaponData : BaseWeapon
    {
        [Space(5)]
        [Header("Weapon Variables")]
        //Type
        public int mRequirement;
        [Space(5)]
        [Header("Ammo")]
        //Ammo Type
        public AttackType type;
        public enum AttackType { KNIFE, BLUNT }
        //Ammo
        public int staminaCost;
        [Space(5)]
        [Header("Damage")]
        //Damage
        public float swingSpeed;
        public float minimumHoldTime;
        public float attackRange;
        [Space(5)]
        [Header("Accuracy")]
        public float missChance;
        public LayerMask attackableLayers;
        [Header("Sound/VFX")]
        //Sound
        public AudioClip swingSound;

    }
}
