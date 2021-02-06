using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RPGSystem.Core.Inventory;
using RPGSystem.Core.Player;
using RPGSystem.VFX;

namespace RPGSystem.Core.Items
{
    [RequireComponent(typeof(AudioSource))]
    public class Firearm : MonoBehaviour
    {
        #region References
        public FirearmWeaponData gunData;
        FirearmWeaponData.FiringType firingType;
        FirearmWeaponData.AmmoType ammoType;

        [HideInInspector]
        public InventoryUIObject uiObject;

        InventoryManager inventoryManager;
        Camera playerCamera;
        [HideInInspector]
        public PlayerManager playerManager;
        public WeaponVFX vfx;
        public CameraVFX camvfx;
        GameObject camContainer;
        AudioSource audioSource;

        public Transform muzzlePointer;
        WeaponUIManager UI_Manager;

        #endregion
        #region UI
        [Header("UI")]
        //Reload progress bar
        public Slider UI_Slider;
        //Current clips ammo
        public TextMeshProUGUI UI_curClipAmmo;
        //Total ammo
        public TextMeshProUGUI UI_reserveAmmo;
        #endregion
        #region Damage Data
        double calcuatedDamage;
        double criticalDamage;
        double playerFirearmSkillLevel;
        double criticalHitChance;
        [HideInInspector]
        public float skillMultiplier;
        #endregion
        #region  Reloading & Ammo
        public int currentAmmo;
        int ammoDeficit;
        [SerializeField]
        int ammoWhenUnequipped;
        public int reserveAmmoOfType;
        float reloadProgress;
        bool canReload;
        bool isReloading = false;
        private float reloadTimeRemaining;
        private const float sliderMax = 1f;
        #endregion
        public bool equipped;
        bool hasBeenInitialized = false;
        bool canShoot = true;
        public bool doesPlayerMeetMinReq = true;
        float accuracyRating;
        float adsAccuracyRating;
        float currentAccuracyRating;

        void Awake()
        {
            gunData.firearmData = gunData;
        }

        public void Equip()
        {
            gunData.isEquipped = true;
            //WaitForFrameEnd();

            if (!hasBeenInitialized)
            {
                InitVariables();
            }
            else
            {
                return;
            }

        }

        public void Unequip()
        {
            uiObject.ammoInClip = currentAmmo;
            ammoDeficit = gunData.clipSize - currentAmmo;
            hasBeenInitialized = true;
            gunData.isEquipped = false;
            uiObject.isEquipped = false;

            UI_reserveAmmo.gameObject.SetActive(false);
            UI_curClipAmmo.gameObject.SetActive(false);
        }

        public void InitVariables()
        {
            //data.script = this;
            //Load all the UI components into their slots from the SO
            UI_Manager = GetComponentInParent<WeaponUIManager>();
            UI_curClipAmmo = UI_Manager.clipAmmo;
            UI_reserveAmmo = UI_Manager.reserveAmmo;
            UI_Slider = UI_Manager.reloadProgress;
            //THIS LOAD ORDER IS WORKING, DO NOT MOVE
            UI_Slider.gameObject.SetActive(false);
            UI_curClipAmmo.gameObject.SetActive(true);
            UI_reserveAmmo.gameObject.SetActive(true);
            UI_curClipAmmo.SetText(currentAmmo.ToString());
            UI_reserveAmmo.SetText(reserveAmmoOfType.ToString());
            UI_curClipAmmo.ForceMeshUpdate();
            UI_reserveAmmo.ForceMeshUpdate();

            //Determine if the player has the correct requirements
            if (playerFirearmSkillLevel >= gunData.fRequirement)
            {
                doesPlayerMeetMinReq = false;
            }
            else if (playerFirearmSkillLevel < gunData.fRequirement)
            {
                doesPlayerMeetMinReq = true;
            }

            inventoryManager = playerManager.GetComponentInParent<InventoryManager>();
            vfx = playerManager.GetComponentInChildren<WeaponVFX>();
            camvfx = playerManager.GetComponentInChildren<CameraVFX>();
            camContainer = GameObject.Find("weaponSway");
            audioSource = GetComponent<AudioSource>();

            //Set enums depending on the SO
            firingType = gunData.firingType;
            ammoType = gunData.ammoType;

            criticalHitChance = gunData.playerData.critChance;
            accuracyRating = gunData.accuracyPenalty;
            adsAccuracyRating = accuracyRating * 0.5f;
            currentAccuracyRating = accuracyRating;

            //Clamp the clip so the player cant keep firing with 0 ammo
            currentAmmo = Mathf.Clamp(currentAmmo, 0, gunData.clipSize);

            playerFirearmSkillLevel = gunData.playerData.fWeapons;

            vfx.kickBackPowerHip = gunData.kickBackPowerHip;
            vfx.kickBackPowerAim = gunData.kickBackPowerAim;
            vfx.ReceiveVectors();

            playerCamera = Camera.main;

            //Rename the prefab to be the same as the SO
            gameObject.name = gunData.itemName;

            //Depending on the AmmoType(determined by the SO), display the Inventorys stock of ammo for that type
            switch (ammoType)
            {
                case FirearmWeaponData.AmmoType.PISTOL:
                    {
                        reserveAmmoOfType = inventoryManager.pistolAmmo;

                        break;
                    }
                case FirearmWeaponData.AmmoType.RIFLE:
                    {
                        reserveAmmoOfType = inventoryManager.rifleAmmo;
                        break;
                    }
                case FirearmWeaponData.AmmoType.SHOTGUN:
                    {
                        reserveAmmoOfType = inventoryManager.shotgunAmmo;
                        break;
                    }
                case FirearmWeaponData.AmmoType.HEAVY:
                    {
                        reserveAmmoOfType = inventoryManager.heavyAmmo;
                        break;
                    }
                case FirearmWeaponData.AmmoType.ENERGY:
                    {
                        reserveAmmoOfType = inventoryManager.energyAmmo;
                        break;
                    }
            }

            Debug.Log("Equiped & initialized " + gunData.itemName);
        }

        void Update()
        {
            #region Keypresses
            if (gunData.isEquipped && !isReloading)
            {
                #region Firing Keypress
                if (Input.GetButtonDown("Fire1") && !inventoryManager.inventoryOpen)
                {
                    if (currentAmmo >= gunData.ammoPerShot && canShoot)
                    {
                        switch (firingType)
                        {
                            case FirearmWeaponData.FiringType.SEMIAUTO:
                                {
                                    StartCoroutine(Shoot());
                                    break;
                                }

                            case FirearmWeaponData.FiringType.BURST:
                                {
                                    StartCoroutine(BurstShoot());
                                    break;
                                }

                            case FirearmWeaponData.FiringType.FULLAUTO:
                                {
                                    StartCoroutine(FullAutoShoot());
                                    break;
                                }
                        }
                    }
                }
                #endregion

                #region Aim
                if (Input.GetButton("Fire2") && !inventoryManager.inventoryOpen)
                {
                    camvfx.aiming = true;
                    currentAccuracyRating = adsAccuracyRating;

                }
                else
                {
                    camvfx.aiming = false;
                    currentAccuracyRating = accuracyRating;
                }
                #endregion

                #region Reloading Keypress
                if (Input.GetButtonDown("Reload") && currentAmmo < gunData.clipSize)
                {
                    ammoDeficit = gunData.clipSize - currentAmmo;

                    if (reserveAmmoOfType >= ammoDeficit)
                    {
                        StartCoroutine(Reload());
                        //Set the timer of the reload cooldown(determined by SO)
                        reloadTimeRemaining = gunData.reloadSpeed;
                    }
                    else if (reserveAmmoOfType == 0)
                    {
                        Debug.Log("Cannot reload, out of reserve ammo");
                    }
                }
                #endregion
            }
            #endregion

            #region Ammo and Reloading UI
            UI_curClipAmmo.SetText(currentAmmo.ToString());
            UI_reserveAmmo.SetText(reserveAmmoOfType.ToString());

            UI_Slider.value = CalculateSliderValue();
            UI_Slider.maxValue = gunData.reloadSpeed;

            if (reloadTimeRemaining <= 0)
            {
                reloadTimeRemaining = 0;
            }
            else if (reloadTimeRemaining > 0)
            {
                reloadTimeRemaining -= Time.deltaTime;
            }
            #endregion
        }

        public void CalculateDamage()
        {
            float randValue = Random.Range(0, 99);

            if (randValue < criticalHitChance)
            {
                calcuatedDamage = gunData.baseDamage * 2;
                Debug.Log("Critical!");
            }
            else
            {
                //Set skill multiplier
                skillMultiplier = ((float)playerFirearmSkillLevel * 0.3f);
                Mathf.RoundToInt(skillMultiplier);

                //Set the calculated damage
                if (doesPlayerMeetMinReq)
                {
                    calcuatedDamage = gunData.baseDamage + (int)skillMultiplier;
                }
                else if (!doesPlayerMeetMinReq)
                {
                    calcuatedDamage = gunData.baseDamage / 2;
                }
            }
        }

        #region Firing
        IEnumerator Shoot()
        {
            canShoot = false;

            InstantiateBullet();
            FiringVFX();
            currentAmmo -= gunData.ammoPerShot;

            yield return new WaitForSeconds(gunData.firingSpeed);
            canShoot = true;
        }

        IEnumerator BurstShoot()
        {
            canShoot = false;

            //Get the index(i) of the amount of rounds per burst(determined by the SO) 
            for (int i = 0; i < gunData.burstSize; i++)
            {
                InstantiateBullet();
                FiringVFX();

                currentAmmo--;

                yield return new WaitForSeconds(gunData.burstInterval);
            }

            yield return new WaitForSeconds(gunData.firingSpeed);
            canShoot = true;
        }

        IEnumerator FullAutoShoot()
        {
            canShoot = false;

            InstantiateBullet();
            FiringVFX();

            currentAmmo -= gunData.ammoPerShot;

            yield return new WaitForSeconds(gunData.firingSpeed);
            canShoot = true;
        }

        void InstantiateBullet()
        {
            //Calculate accuracy (determined by SO)
            float width = Random.Range(-1f, 1f) * -currentAccuracyRating;
            float height = Random.Range(-1f, 1f) * -currentAccuracyRating;

            //Create a clone instance of the bullet at the firing position(currently the camera, this will change later)
            GameObject bulletInstance = Instantiate(gunData.bullet, muzzlePointer.position, muzzlePointer.rotation);

            Bullet bulletComponent = bulletInstance.GetComponent<Bullet>();
            CalculateDamage();

            bulletComponent.damage = calcuatedDamage;
            bulletComponent.maxRange = gunData.projectileRange;

            //Get the Rigidbody component of the bullet and apply a force to it (determined by the SO)
            bulletInstance.GetComponent<Rigidbody>().AddForce(transform.forward * gunData.projectileSpeed + transform.right * width + transform.up * height, ForceMode.Impulse);
        }

        void FiringVFX()
        {
            camvfx.Fire();
            vfx.Fire();

            audioSource.PlayOneShot(gunData.firingSFX);
            audioSource.pitch = Random.Range(0.9f, 1.05f);
            audioSource.panStereo = Random.Range(-0.03f, 0.03f);

            GameObject flashClone = Instantiate(gunData.muzzleFlash, muzzlePointer.position, muzzlePointer.rotation);
            flashClone.AddComponent<DestroyAfterTime>().lifespan = 0.1f;
        }
        #endregion

        #region Reloading
        IEnumerator Reload()
        {
            UI_Slider.gameObject.SetActive(true);

            canShoot = false;
            isReloading = true;

            currentAmmo += ammoDeficit;
            reserveAmmoOfType -= ammoDeficit;
            SubtractAmmoFromInventory();
            ammoDeficit = 0;

            yield return new WaitForSeconds(gunData.reloadSpeed);
            UI_Slider.gameObject.SetActive(false);

            isReloading = false;
            canShoot = true;
        }
        #endregion

        void SubtractAmmoFromInventory()
        {
            switch (ammoType)
            {
                case FirearmWeaponData.AmmoType.PISTOL:
                    {
                        inventoryManager.pistolAmmo -= ammoDeficit;

                        break;
                    }
                case FirearmWeaponData.AmmoType.RIFLE:
                    {
                        inventoryManager.rifleAmmo -= ammoDeficit;
                        break;
                    }
                case FirearmWeaponData.AmmoType.SHOTGUN:
                    {
                        inventoryManager.shotgunAmmo -= ammoDeficit;
                        break;
                    }
                case FirearmWeaponData.AmmoType.HEAVY:
                    {
                        inventoryManager.heavyAmmo -= ammoDeficit;
                        break;
                    }
                case FirearmWeaponData.AmmoType.ENERGY:
                    {
                        inventoryManager.energyAmmo -= ammoDeficit;
                        break;
                    }
            }
        }

        float CalculateSliderValue()
        {
            //Divides the reloadTime by the Sliders max value(1) to return 1 when the timer is at full
            return (reloadTimeRemaining / sliderMax);
        }
    }
}
