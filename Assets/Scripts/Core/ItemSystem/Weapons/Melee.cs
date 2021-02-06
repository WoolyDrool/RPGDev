using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPGSystem.Core.Inventory;
using RPGSystem.Core.Player;
using RPGSystem.VFX;

namespace RPGSystem.Core.Items
{
    public class Melee : MonoBehaviour
    {
        public MeleeWeaponData weaponData;
        MeleeWeaponData.AttackType weaponType;

        [HideInInspector]
        public InventoryUIObject uiObject;

        InventoryManager inventoryManager;
        Camera playerCamera;
        PlayerManager playerData;
        AudioSource audioSource;
        public WeaponVFX vfx;
        public CameraVFX camvfx;

        [HideInInspector]
        public Slider UI_Slider;
        WeaponUIManager UI_Manager;

        //Final calculated damage
        double calcDamage;
        //Critical damage;
        double critDamage;
        //Firerarms skill level
        double playerMeleeSkillLevel;
        //Final calculated skillMultiplier (public so other classes can access it, but hidden in inspector for clarity)
        double criticalHitChance;

        Ray damageRay;
        RaycastHit damageHit;

        [HideInInspector]
        public float skillMultiplier;

        //Switch for if the gun is being held
        public bool equipped;
        //Switch for shooting state
        public bool canSwing = true;
        //Critical hit
        bool isCrit = false;
        public bool doesPlayerMeetMinReq = true;
        public float chargeCounter;



        void Awake()
        {
            playerData = GameObject.FindWithTag("Player").GetComponent<PlayerManager>();
            weaponData.meleeWeaponData = weaponData;
        }

        public void Equip()
        {
            InitVariables();
            equipped = true;
        }

        public void Unequip()
        {
            equipped = false;
            weaponData.isEquipped = false;
        }

        void InitVariables()
        {

            UI_Manager = GetComponentInParent<WeaponUIManager>();
            UI_Slider = UI_Manager.reloadProgress;
            //Find the instances of the Inventory and CharacterSheet scripts, which will always be attached to the player
            inventoryManager = playerData.GetComponentInParent<InventoryManager>();
            //Find the audio source for the gun
            audioSource = GetComponent<AudioSource>();

            //Force an update of the UI components 
            //Set enums depending on the SO
            weaponType = weaponData.type;

            criticalHitChance = weaponData.playerData.critChance;

            playerMeleeSkillLevel = weaponData.playerData.mWeapons;

            //Get the MainCamera GO
            playerCamera = Camera.main;

            //Rename the prefab to be the same as the SO
            gameObject.name = weaponData.itemName;

            Debug.Log("Initialized " + weaponData.itemName);

            //Find the VFX related components
            vfx = playerData.GetComponentInChildren<WeaponVFX>();
            camvfx = playerData.GetComponentInChildren<CameraVFX>();
        }

        // Update is called once per frame
        void Update()
        {
            chargeCounter = Mathf.Clamp(chargeCounter, 0, 100);
            damageRay = playerCamera.ScreenPointToRay(Input.mousePosition);
            if (equipped && canSwing)
            {
                #region Firing Keypress
                //Check that the player has ammo
                if (Input.GetButton("Fire1") && !inventoryManager.inventoryOpen)
                {
                    chargeCounter += 50 * Time.deltaTime;
                }
                else if (Input.GetButtonUp("Fire1") && canSwing)
                {
                    if (chargeCounter < 100)
                    {
                        StartCoroutine(NormalSwing());
                    }
                    else if (chargeCounter >= 100)
                    {
                        StartCoroutine(HeldSwing());
                        Debug.Log("Critical swing!");
                    }
                }
                #endregion
            }

            if (chargeCounter > 100)
            {
                UI_Slider.gameObject.SetActive(true);
            }
        }

        void CalculateDamage()
        {
            float randValue = Random.Range(0, 99);
            if (randValue < criticalHitChance)
            {
                isCrit = true;
            }
            else
            {
                isCrit = false;
            }

            if (isCrit)
            {
                calcDamage = weaponData.baseDamage * 2;
                Debug.Log("Critical!");
            }
            else
            {
                //Set skill multiplier
                skillMultiplier = ((float)playerMeleeSkillLevel * 0.3f);
                Mathf.RoundToInt(skillMultiplier);

                //Set the calculated damage
                if (doesPlayerMeetMinReq)
                {
                    calcDamage = weaponData.baseDamage + (int)skillMultiplier;
                }
                else if (!doesPlayerMeetMinReq)
                {
                    calcDamage = weaponData.baseDamage / 2;
                }

            }

            critDamage = calcDamage * 2;
        }

        IEnumerator NormalSwing()
        {
            chargeCounter = 0;
            //Disable players ability to shoot again until complete       
            canSwing = false;
            //Calculate damage
            CalculateDamage();
            SwingVFX();

            if (Physics.Raycast(damageRay, out damageHit, weaponData.attackRange, weaponData.attackableLayers))
            {
                damageHit.collider.SendMessage("TakeDamage", calcDamage);
                Debug.Log("Hit an enemy for  " + calcDamage.ToString() + " damage");
            }
            //Wait until the firing state is complete before letting the player fire again
            yield return new WaitForSeconds(weaponData.swingSpeed);
            canSwing = true;

        }

        IEnumerator HeldSwing()
        {
            //Disable players ability to shoot again until complete       
            canSwing = false;
            //Calculate damage
            SwingVFX();
            CalculateDamage();
            if (Physics.Raycast(damageRay, out damageHit, weaponData.attackRange, weaponData.attackableLayers))
            {
                damageHit.collider.SendMessage("TakeDamage", critDamage);
                Debug.Log("Hit an enemy for  " + critDamage.ToString() + " damage");
            }
            chargeCounter = 0;
            //Wait until the firing state is complete before letting the player fire again
            yield return new WaitForSeconds(weaponData.swingSpeed);
            UI_Slider.gameObject.SetActive(false);
            canSwing = true;
        }

        IEnumerator IncreaseSlider()
        {
            UI_Slider.value += chargeCounter / 100;
            yield return new WaitForSeconds(1);
        }

        void SwingVFX()
        {
            camvfx.Fire();
            vfx.Fire();

            audioSource.PlayOneShot(weaponData.swingSound);
            audioSource.pitch = Random.Range(0.9f, 1.05f);
            audioSource.panStereo = Random.Range(-0.03f, 0.03f);
        }

        float CalculateSliderValue()
        {
            return (chargeCounter / 1);
        }
    }
}
