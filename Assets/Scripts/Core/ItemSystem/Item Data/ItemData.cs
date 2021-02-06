using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGSystem.Core.Items
{
    [System.Serializable]
    public class ItemData : ScriptableObject
    {
        public Firearm firearmScript;
        public Melee meleeScript;
        public Consumable consumableScript;
        [Header("Data")]
        //Flavor Text
        public string itemName;
        [TextArea]
        public string itemDescription;
        [TextArea]
        public string flavorText;
        //PLANNED: public Sprite weaponIcon;
        public ItemType itemType;
        public enum ItemType { FIREARM, MELEEWEAPON, CONSUMABLE }
        public string itemID;
        public int itemWeight;
        public GameObject sceneInstance;
        //public GameObject sceneInstance;
        public bool spawnedAsItem = false;
        public PlayerData playerData;
        public GameObject modelWhenDropped;
    }
}
