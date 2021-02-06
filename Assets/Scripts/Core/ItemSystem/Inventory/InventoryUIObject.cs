using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using RPGSystem.Core.Items;
using TMPro;

namespace RPGSystem.Core.Inventory
{
    public class InventoryUIObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [Header("Item Variables")]
        public string itemName;
        public string itemDescription;
        public string flavorText;
        public string value;
        public string value2;
        public string firingType;
        public string clipSize;
        public string skillMultiplier;
        public string itemWeight;
        public int ammoInClip;
        public int lastFiredAmmo;
        public int usesBeforeGone = 1;

        public BaseWeapon weaponReference;
        public BaseConsumable consumableReference;
        [HideInInspector]
        public InventoryManager inventory;
        public bool isEquipped = false;
        [HideInInspector]
        public Button buttonComponent;
        public Firearm fComponent;
        public Melee mComponent;
        public Consumable conComponent;

        void Start()
        {
            //Grab components
            inventory = GetComponentInParent<InventoryManager>();
            buttonComponent = GetComponent<Button>();
            buttonComponent.interactable = true;
            inventory.descriptionBox.SetText("");
            inventory.valueText.SetText("");
            inventory.value2Text.SetText("");
            inventory.skText.SetText("");
            inventory.modeText.SetText("");
            inventory.clipText.SetText("");
            inventory.flavorText.SetText("");
            inventory.itemWeightText.SetText("");

            if (fComponent)
            {
                fComponent.uiObject = this;
            }
            else if (mComponent)
            {
                mComponent.uiObject = this;
            }
        }

        void UpdateStatus()
        {
            isEquipped = weaponReference.isEquipped;
        }

        public void OnPointerEnter(PointerEventData data)
        {
            inventory.descriptionBox.SetText(itemDescription);
            inventory.flavorText.SetText(flavorText);
            inventory.itemWeightText.SetText("WGT: " + itemWeight);
            if (fComponent)
            {
                //When the player hovers over, set UI variables

                inventory.valueText.SetText("DPS: " + value);
                inventory.value2Text.SetText("AT: " + value2);
                inventory.skText.SetText("SK:" + skillMultiplier);
                inventory.modeText.SetText("FT: " + firingType);
                inventory.clipText.SetText("CA: " + clipSize);
            }
            else if (mComponent)
            {
                //When the player hovers over, set UI variables
                inventory.valueText.SetText("DPS: " + value);
                inventory.skText.SetText("SK:" + skillMultiplier);
                inventory.value2Text.SetText("");
                inventory.modeText.SetText("");
                inventory.clipText.SetText("");
            }
            else if (conComponent)
            {
                inventory.valueText.SetText("++: " + value);
                inventory.value2Text.SetText("--:" + value2);
                inventory.skText.SetText("");
                inventory.modeText.SetText("");
                inventory.clipText.SetText("");

            }
        }

        public void OnPointerExit(PointerEventData data)
        {
            //When the player hovers off, clear the UI variables
            inventory.descriptionBox.SetText("");
            inventory.valueText.SetText("");
            inventory.value2Text.SetText("");
            inventory.skText.SetText("");
            inventory.modeText.SetText("");
            inventory.clipText.SetText("");
            inventory.flavorText.SetText("");
            inventory.itemWeightText.SetText("");
        }

        public void OnPointerClick(PointerEventData data)
        {
            if (data.button == PointerEventData.InputButton.Left)
            {
                if (weaponReference)
                {
                    EquipFromMenu();
                }
                else if (consumableReference)
                {
                    Debug.Log("Equipping consumable");
                    UseFromMenu();
                }

            }
            else if (data.button == PointerEventData.InputButton.Right)
            {
                if (isEquipped)
                {
                    if (weaponReference)
                    {
                        Debug.Log("Unequipping");
                        UnequipFromMenu();
                        isEquipped = false;
                    }
                    else if (consumableReference)
                    {
                        return;
                    }

                }
            }

            if (data.button == PointerEventData.InputButton.Middle)
            {
                if (!isEquipped)
                {
                    if (consumableReference != null)
                    {
                        DropConsumable();
                    }
                    else if (weaponReference != null)
                    {
                        DropWeapon();
                    }
                }

            }
        }

        #region Weapons
        public void EquipFromMenu()
        {
            isEquipped = false;
            inventory.EquipWeapon(weaponReference);
            isEquipped = true;
            UpdateStatus();
        }

        public void UnequipFromMenu()
        {
            inventory.UnequipWeapon();
            isEquipped = false;
            UpdateStatus();
        }

        public void DropWeapon()
        {
            inventory.DropItem(weaponReference, this);
            UpdateStatus();

        }
        #endregion

        #region Consumables

        public void UseFromMenu()
        {
            conComponent.OnUse();
            inventory.UseItem(consumableReference, this);
        }

        public void DropConsumable()
        {
            inventory.DropItem(consumableReference, this);
        }

        #endregion
    }
}
