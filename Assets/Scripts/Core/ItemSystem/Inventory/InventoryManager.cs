using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using RPGSystem.Core.Items;
using RPGSystem.Core.Player;

namespace RPGSystem.Core.Inventory
{
    //Disable a warning message that means nothing
#pragma warning disable 0414
    public class InventoryManager : MonoBehaviour
    {
        public bool inventoryOpen = false;

        PlayerCharacterSheet playerCharacterSheet;
        PlayerManager playerManager;

        #region Item Pick Up/Drop
        //In scene GameObject reference for item pickups
        GameObject aquiredObject;

        //Sorting: The items base data class
        ItemData itemData;

        //Sorting: temporary data for the corresponding item type
        BaseWeapon tempWeaponData;
        BaseConsumable tempConsumableData;

        [Space(5)]
        [Header("Item Pickup")]
        public WeaponManager weaponManager;
        //The base file for item drops
        public GameObject ItemDropMaster;

        //Dropped item spawn point in world
        public Transform itemDropPoint;
        #endregion

        [Space(5)]
        [Header("Raycast")]
        #region Raycast
        RaycastHit hit;
        Ray ray;
        float rayRange = 1.7f;
        public LayerMask pickingLayerMask;
        #endregion

        #region Global Variables
        [Space(5)]
        [Header("Inventory")]
        //Players current carry weight
        public int currentWeight = 0;
        //Maxium possible carry weight
        int maxCarryWeight = 250;
        #endregion

        //The global list of all items 
        public List<ItemData> GlobalInventory;

        [Space(5)]
        [Header("Weapons")]
        public List<ItemData> inventoryWeaponsList = new List<ItemData>();

        [Space(5)]
        [Header("Consumables")]
        public List<ItemData> inventoryConsumablesList = new List<ItemData>();

        #region Item Variables
        [Space(5)]
        [Header("Ammo")]
        //This is going to change
        public int pistolAmmo = 0;
        public int rifleAmmo = 0;
        public int shotgunAmmo = 0;
        public int heavyAmmo = 0;
        public int energyAmmo = 0;
        #endregion

        #region UI
        [Space(5)]
        [Header("UI - Main")]
        public List<GameObject> uiObjectList = new List<GameObject>();
        //The hover text, might change this to be a loaded reference instead of a in Scene object
        public TextMeshProUGUI itemHoverText;
        public Transform gridLayoutGroup;
        public GameObject InventoryUIPanel;
        public Button InventoryUIObject;

        [Space(5)]
        [Header("UI - Flavor Text")]
        public TextMeshProUGUI descriptionBox;
        public TextMeshProUGUI flavorText;

        public TextMeshProUGUI valueText;
        public TextMeshProUGUI value2Text;

        public TextMeshProUGUI skText;
        public TextMeshProUGUI modeText;
        public TextMeshProUGUI clipText;

        public TextMeshProUGUI carryWeightText;
        public TextMeshProUGUI itemWeightText;
        #endregion

        void Awake()
        {
            playerCharacterSheet = GetComponent<PlayerManager>().playerCharacterSheet;
            playerManager = GetComponent<PlayerManager>();

            InventoryUIPanel.SetActive(false);

            #region ClampAmmo
            pistolAmmo = Mathf.Clamp(pistolAmmo, 0, 999);
            rifleAmmo = Mathf.Clamp(rifleAmmo, 0, 999);
            shotgunAmmo = Mathf.Clamp(shotgunAmmo, 0, 999);
            heavyAmmo = Mathf.Clamp(heavyAmmo, 0, 999);
            energyAmmo = Mathf.Clamp(energyAmmo, 0, 999);
            #endregion

            //Start with the hover text inactive, again this might change 
            itemHoverText.gameObject.SetActive(false);

            //Clear Strings
            descriptionBox.SetText("");
            valueText.SetText("");
            skText.SetText("");
            value2Text.SetText("");
            modeText.SetText("");
            clipText.SetText("");
            carryWeightText.SetText("");
            itemWeightText.SetText("");
            flavorText.SetText("");
        }

        public void Update()
        {
            //Set the ray to be at the mouses position on the screen
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            #region Pickup Ray
            if (Physics.Raycast(ray, out hit, rayRange, pickingLayerMask))
            {
                if (hit.collider.tag == "Item")
                {
                    aquiredObject = hit.collider.gameObject;

                    itemData = aquiredObject.GetComponent<ItemPickup>().data;

                    //Enable the hover text
                    itemHoverText.gameObject.SetActive(true);
                    itemHoverText.SetText("Pick Up " + "\"" + itemData.itemName + "\"");

                    if (Input.GetButtonDown("General Interact"))
                    {
                        if (currentWeight + itemData.itemWeight < maxCarryWeight)
                        {
                            //Set the in scene reference to inactive
                            hit.collider.gameObject.SetActive(false);

                            //Add the item to the inventory
                            SortItem(itemData);

                            return;
                        }
                        else
                        {
                            Debug.LogError("Could not pick up weapon, too heavy");
                        }
                    }
                }
            }
            else
            {
                itemHoverText.gameObject.SetActive(false);

                aquiredObject = null;
                itemData = null;
            }
            #endregion

            if (Input.GetKeyDown(KeyCode.I))
            {
                OpenUI();
            }
        }

        public void UpdateGlobalInventory()
        {
            GlobalInventory = inventoryWeaponsList.Union<ItemData>(inventoryConsumablesList).ToList<ItemData>();
        }

        void SortItem(ItemData itemToSort)
        {
            //Get the items weight and add it to the variable
            int weightToAdd = itemToSort.itemWeight;
            currentWeight += weightToAdd;

            //Check what type of item the itemToSort is
            switch (itemToSort.itemType)
            {
                //Firearm
                case ItemData.ItemType.FIREARM:
                    {
                        //set the itemPickup to use the items dataPrefab
                        aquiredObject = itemToSort.sceneInstance;
                        tempWeaponData = itemToSort.firearmScript.gunData;
                        itemToSort.firearmScript.playerManager = playerManager;
                        AddWeapon(tempWeaponData);
                        inventoryWeaponsList.Add(itemToSort);
                        break;
                    }

                //Melee
                case ItemData.ItemType.MELEEWEAPON:
                    {
                        aquiredObject = itemToSort.sceneInstance;
                        tempWeaponData = itemToSort.meleeScript.weaponData;
                        AddWeapon(tempWeaponData);
                        inventoryWeaponsList.Add(itemToSort);
                        break;
                    }

                //Consumable
                case ItemData.ItemType.CONSUMABLE:
                    {
                        aquiredObject = itemToSort.sceneInstance;
                        tempConsumableData = itemToSort.consumableScript.consumableData;
                        itemToSort.consumableScript.playerManager = playerManager;
                        AddConsumable(tempConsumableData);
                        inventoryConsumablesList.Add(itemToSort);
                        break;
                    }
            }

            //Apply a movement speed penalty to the player after roughly 75% carry weight
            if (currentWeight >= (int)(maxCarryWeight / 1.15f))
            {
                playerManager.TakeSpeedPenalty(true);
                carryWeightText.color = Color.red;
            }
            else
            {
                playerManager.TakeSpeedPenalty(false);
                carryWeightText.color = Color.white;
            }
        }

        public void AddWeapon(BaseWeapon weapon)
        {
            //Add the UI component to the Canvas
            GameObject weaponInUIObject = Instantiate<GameObject>(InventoryUIObject.gameObject);
            weaponInUIObject.transform.SetParent(gridLayoutGroup.transform);
            weaponInUIObject.GetComponentInChildren<TextMeshProUGUI>().SetText(weapon.itemName);

            //Grab the component for the UI object
            InventoryUIObject invObjectUI = weaponInUIObject.GetComponent<InventoryUIObject>();

            uiObjectList.Add(weaponInUIObject);

            tempWeaponData = null;

            //Set UIObject variables
            invObjectUI.itemName = weapon.itemName;
            invObjectUI.itemDescription = weapon.itemDescription;
            invObjectUI.weaponReference = weapon;
            invObjectUI.value = weapon.baseDamage.ToString();
            invObjectUI.flavorText = weapon.flavorText;
            invObjectUI.name = weapon.itemName;
            invObjectUI.itemWeight = weapon.itemWeight.ToString();

            if (weapon.itemType == ItemData.ItemType.FIREARM)
            {
                invObjectUI.fComponent = weapon.firearmScript;
                invObjectUI.mComponent = null;
                invObjectUI.value2 = weapon.firearmData.ammoType.ToString();
                invObjectUI.firingType = weapon.firearmData.firingType.ToString();
                invObjectUI.clipSize = weapon.firearmData.clipSize.ToString();
                invObjectUI.skillMultiplier = weapon.firearmScript.skillMultiplier.ToString();
            }
            else if (weapon.itemType == ItemData.ItemType.MELEEWEAPON)
            {
                invObjectUI.mComponent = weapon.meleeScript;
                invObjectUI.fComponent = null;
                invObjectUI.skillMultiplier = weapon.meleeScript.skillMultiplier.ToString();
            }
        }

        public void AddConsumable(BaseConsumable consumable)
        {
            //Add UI component to the Canvas
            GameObject consumableInUIObject = Instantiate<GameObject>(InventoryUIObject.gameObject);
            consumableInUIObject.transform.SetParent(gridLayoutGroup.transform);
            consumableInUIObject.GetComponentInChildren<TextMeshProUGUI>().SetText(consumable.itemName);

            //Grab the component for the UI object
            InventoryUIObject invObjectUI = consumableInUIObject.GetComponent<InventoryUIObject>();

            uiObjectList.Add(consumableInUIObject);

            tempConsumableData = null;

            //Set UIObject variables
            invObjectUI.itemName = consumable.itemName;
            invObjectUI.consumableReference = consumable;
            invObjectUI.itemName = consumable.itemName;
            invObjectUI.itemDescription = consumable.itemDescription;
            invObjectUI.flavorText = consumable.flavorText;
            invObjectUI.name = consumable.itemName;
            invObjectUI.itemWeight = consumable.itemWeight.ToString();
            invObjectUI.conComponent = consumable.consumableScript;
        }

        #region UI
        public void OpenUI()
        {
            Time.timeScale = 0;
            //Set the panel active
            InventoryUIPanel.SetActive(true);

            //Set carryweight text
            carryWeightText.SetText(currentWeight.ToString() + "/" + maxCarryWeight.ToString());

            //Freeze the game and lock the cursor
            playerManager.gameStateManager.FreezeGame();
            playerManager.gameStateManager.RevealMouse();

            UpdateGlobalInventory();
            //Inventory is open
            inventoryOpen = true;
        }

        public void CloseUI()
        {
            Time.timeScale = 1;
            //Set the panel to inactive
            InventoryUIPanel.SetActive(false);

            //Resume the game and hide the mouse
            playerManager.gameStateManager.ResumeGame();
            playerManager.gameStateManager.HideMouse();

            //Inventory is closed
            inventoryOpen = false;
        }
        #endregion

        #region Weapon Specific Functions
        public void EquipWeapon(BaseWeapon weaponData)
        {
            //Send the data to the WeaponManager object
            weaponManager.ReceiveWeapon(weaponData);
            //Give the gun the players data
            weaponData.playerData = playerCharacterSheet.playerSaveState;
        }

        public void UnequipWeapon()
        {
            weaponManager.RemoveWeapon();
        }

        public void SwitchWeapon()
        {
            weaponManager.SwitchWeapon();
        }
        #endregion

        public void DropItem(ItemData itemToRemove, InventoryUIObject itemUI)
        {

            Debug.Log("Dropped " + itemToRemove.itemName);
            GameObject droppedItem = Instantiate(ItemDropMaster, itemDropPoint.position, itemDropPoint.rotation);
            //droppedItem.AddComponent<ItemPickup>();
            droppedItem.GetComponent<ItemPickup>().data = itemToRemove;
            droppedItem.GetComponent<ItemPickup>().refModel = itemToRemove.modelWhenDropped;
            droppedItem.name = itemToRemove.name + "(Dropped)";
            //droppedItem.AddComponent<ItemPickup>().data = itemToRemove;
            GlobalInventory.Remove(itemToRemove);
            inventoryWeaponsList.Remove(itemToRemove);
            uiObjectList.Remove(itemUI.gameObject);
            Destroy(itemUI.gameObject);
        }

        public void UseItem(ItemData itemToUse, InventoryUIObject itemUI)
        {
            GlobalInventory.Remove(itemToUse);
            uiObjectList.Remove(itemUI.gameObject);
            Destroy(itemUI.gameObject);
        }
    }
}
