using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGSystem.Core.Items
{
    [CreateAssetMenu(menuName = "Items/Consumable")]
    public class BaseConsumable : ItemData
    {
        public ConsumableType consumableType;
        public enum ConsumableType { HEALTH, STAMINA, SANITY, XP, STAT, OTHER }

        [Header("Health")]
        public double healthPositiveGain;
        public double healthNegativeGain;

        [Header("Stamina")]
        public double staminaPositiveGain;
        public double staminaNegativeGain;

        [Header("Sanity")]
        public double sanityPositiveGain;
        public double sanityNegativeGain;
    }
}
