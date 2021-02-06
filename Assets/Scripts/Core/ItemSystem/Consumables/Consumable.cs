using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPGSystem.Core.Inventory;
using RPGSystem.Core.Player;

namespace RPGSystem.Core.Items
{
    public class Consumable : MonoBehaviour
    {
        public BaseConsumable consumableData;
        [HideInInspector]
        public InventoryUIObject uiObject;
        InventoryManager inventoryManager;
        [HideInInspector]
        public PlayerManager playerManager;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnUse()
        {
            playerManager.currentHealth += (float)consumableData.healthPositiveGain;
            playerManager.currentHealth -= (float)consumableData.healthNegativeGain;

            playerManager.stamina += (float)consumableData.staminaPositiveGain;
            playerManager.stamina -= (float)consumableData.staminaNegativeGain;

            playerManager.sanity += (float)consumableData.sanityPositiveGain;
            playerManager.sanity -= (float)consumableData.sanityNegativeGain;
        }
    }
}
